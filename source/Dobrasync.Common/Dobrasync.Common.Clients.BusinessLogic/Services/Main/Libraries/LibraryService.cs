using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dobrasync.Common.Clients.Api;
using Dobrasync.Common.Clients.BusinessLogic.CObj;
using Dobrasync.Common.Clients.Database.DB.Entities;
using Dobrasync.Common.Clients.Database.Repos;
using Dobrasync.Common.Util;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using File = Dobrasync.Common.Clients.Database.DB.Entities.File;
using Version = Dobrasync.Common.Clients.Database.DB.Entities.Version;

namespace Dobrasync.Common.Clients.BusinessLogic.Services.Main.Libraries;

public class LibraryService(IApiClient api, IRepoWrapper repo) : ILibraryService
{
    public async Task CloneLibraryAsync(Guid remoteId, string path)
    {
        if (repo.LibraryRepo.QueryAll().Any(x => x.RemoteId == remoteId))
        {
            return;
        }
        
        LibraryDto remoteLibrary = await api.GetLibraryByIdAsync(remoteId);
        Library created = await CreateLibraryLocal(remoteLibrary.Id, path);
    }

    public async Task<LibraryDto> CreateLibraryAsync(string name)
    {
        LibraryDto created = await api.CreateLibraryAsync(new()
        {
            Name = name,
        });
        
        return created;
    }

    public async Task SyncLibraryAsync(IProgress<SyncProgressUpdateBase> progress, CancellationToken cancellationToken, Guid libraryId)
    {
        #region Load library
        Library? library = await repo.LibraryRepo
            .QueryAll()
            .FirstOrDefaultAsync(x => x.Id == libraryId);

        if (library == null)
        {
            progress.Report(new SyncPUFatalLibraryNotFound());
            return;
        }
        #endregion
        #region Build file tree
        progress.Report(new SyncPUBuildingFileTree());
        FileTreeBuildResult tree = await CreateAndUpdateLocalLibraryFileTree(library.Id);
        #endregion
        #region Get diff
        progress.Report(new SyncPUFetchingDiff());
        ICollection<string> diff = await api.MakeLibraryDiffAsync(libraryId, new()
        {
            FilesOnLocal = tree.FilesAll.Select(x => new DiffFileDescriptionDto()
            {
                Path = x.Path,
                FileChecksum = ChecksumUtil.CalculateFileChecksum(
                    System.IO.File.ReadAllBytes(tree.FilesInfo.Find(fi => Path.GetRelativePath(library.Path, fi.FullName) == x.Path)!.FullName)
                ),
                LatestVersionId = x.Versions.OrderByDescending(v => v.CreatedUtc).ToList().First().RemoteId,
            }).ToList()
        });
        #endregion
        
        #region Build Pull/Push list
        progress.Report(new SyncPUBuildingPushPullLists());
        HashSet<FileWithVersionInfo> filesToPush = new();
        HashSet<FileWithVersionInfo> filesToPull = new();
        HashSet<FileWithVersionInfo> filesUndecided = new();
        HashSet<FileWithVersionInfo> filesFailure = new();
        
        foreach (string filepath in diff)
        {
            File? localFile = await repo.FileRepo
                .QueryAll()
                .Where(x => x.LibraryId == libraryId)
                .Include(x => x.Versions)
                .FirstOrDefaultAsync(x => x.Path == filepath);

            #region Pull - If local doesnt have match
            if (localFile == null)
            {
                filesToPull.Add(new()
                {
                    Path = filepath,
                    LatestVersion = null
                });
                continue;
            }
            #endregion
            #region Push - if remote doesnt have match
            FileDto? apiFileInfo = null;
            try
            {
                apiFileInfo = await api.GetLibraryFileAsync(libraryId, filepath);
            }
            catch (ApiException apiEx)
            {
                if (apiEx.StatusCode == 404)
                {
                    filesToPush.Add(new()
                    {
                        Path = filepath,
                        LatestVersion = null
                    });
                    continue;
                }
            }
            #endregion
            #region Fail - if remote returned unknown error
            if (apiFileInfo == null)
            {
                filesFailure.Add(new()
                {
                    Path = filepath,
                    LatestVersion = null,
                });
                continue;
            }
            #endregion

            VersionDto latestFileVersion = await api.GetVersionRequiredAsync(apiFileInfo.CurrentVersionId);
            #region Pull - if local has no versions
            if (localFile.Versions.Count == 0)
            {
                filesToPull.Add(new()
                {
                    Path = filepath,
                    LatestVersion = apiFileInfo.CurrentVersionId
                });
                continue;
            }
            #endregion
            
            // in case of conflict, let user decide if the local version or remote version should be used
            if (latestFileVersion.Id != localFile.Versions.OrderByDescending(x => x.CreatedUtc).First().RemoteId)
            {
                filesToPush.Add(new()
                {
                    Path = filepath,
                    LatestVersion = apiFileInfo.CurrentVersionId
                });
                continue;
            }

            throw new ArgumentException("Unexpected condition");
        }
        #endregion
        
        #region Pull
        foreach (var file in filesToPull)
        {
            progress.Report(new SyncPUPullingFile(file.Path));
            if (file.LatestVersion == null) continue;
            
            ICollection<string> blocks = await api.GetVersionBlocksAsync(file.LatestVersion.Value);
            List<byte> allBytes = new();
            foreach (var block in blocks)
            {
                BlockDto payload = await api.GetBlockAsync(library.RemoteId, block);
                allBytes.AddRange(payload.Payload);
            }

            string fileSystemPath = Path.Combine(library.Path, file.Path);
            await System.IO.File.WriteAllBytesAsync(fileSystemPath, allBytes.ToArray());
        }
        #endregion
        #region Push
        foreach (var file in filesToPush)
        {
            progress.Report(new SyncPUPushingFile(file.Path));
            
            #region Gather local file facts
            string fileSystemPath = Path.Combine(library.Path, file.Path);
            FileInfo fileInfo = new(fileSystemPath);
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(fileSystemPath);
            string fileChecksum = ChecksumUtil.CalculateFileChecksum(fileBytes);
            List<byte[]> allBlocks = Chunker.ContentToBlocks(fileBytes);
            List<string> allBlocksChecksums = allBlocks.Select(ChecksumUtil.CalculateBlockChecksum).ToList();
            #endregion
            #region Create new version
            VersionCreateResultDto newVersion = await api.CreateVersionAsync(new()
            {
                FileChecksum = fileChecksum,
                LibraryId = library.Id,
                ExpectedBlocks = allBlocks.Select(x => ChecksumUtil.CalculateFileChecksum(x)).ToList(),
                FilePath = file.Path,
                IsDirectory = (fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory,
                FilePermissionsOctal = 0,
                FileCreatedOnUtc = fileInfo.CreationTimeUtc,
                FileModifiedOnUtc = fileInfo.LastWriteTimeUtc,
            });
            #endregion
            #region Send required blocks
            foreach (string requiredBlock in newVersion.RequiredBlocks)
            {
                int blockMatchIndex = allBlocksChecksums.IndexOf(requiredBlock);
                byte[] blockMatchPayload = allBlocks[blockMatchIndex];
                
                await api.CreateBlockAsync(library.RemoteId, new()
                {
                    Payload = blockMatchPayload,
                    Checksum = requiredBlock,
                });
            }
            #endregion
            #region Complete version
            await api.CompleteVersionAsync(newVersion.CreatedVersion.Id);
            #endregion
        };
        #endregion
        
        progress.Report(new SyncPUComplete());
    }

    private async Task<FileTreeBuildResult> CreateAndUpdateLocalLibraryFileTree(Guid libraryId)
    {
        Library library = await repo.LibraryRepo
            .QueryAll()
            .FirstAsync(x => x.Id == libraryId);

        string libPath = library.Path;
        string[] filesWithinLibrary = Directory.GetFileSystemEntries(libPath);
        
        List<FileInfo> files = new List<FileInfo>();
        foreach (string filepath in filesWithinLibrary)
        {
            FileInfo fileInfo = new FileInfo(filepath);
            if (fileInfo.Exists)
            {
                files.Add(fileInfo);
            }
        }
        
        return await UpdateLibraryFileTree(libraryId, files);
    }

    private async Task<FileTreeBuildResult> UpdateLibraryFileTree(Guid libraryId, List<FileInfo> fileInfoList)
    {
        Library library = repo.LibraryRepo.QueryAll().First(x => x.Id == libraryId);
        
        List<File> newFiles = new();
        List<File> changedFiles = new();
        List<File> untouchedFiles = new();
        
        foreach (var fileInfo in fileInfoList)
        {
            string filePathInLibrary = Path.GetRelativePath(library.Path, fileInfo.FullName);

            File? trackedFile = await repo.FileRepo
                .QueryAll()
                .Include(x => x.Versions)
                .FirstOrDefaultAsync(x => x.Path == filePathInLibrary);

            if (trackedFile == null)
            {
                trackedFile = new File()
                {
                    Path = filePathInLibrary,
                };
                newFiles.Add(trackedFile);
                continue;
            }

            if (trackedFile.Versions?.Count == 0)
            {
                untouchedFiles.Add(trackedFile);
                continue;
            }
            
            byte[] fileBytesLocal = await System.IO.File.ReadAllBytesAsync(filePathInLibrary);
            string currentLocalFilesChecksum = ChecksumUtil.CalculateFileChecksum(fileBytesLocal);
            if (trackedFile.Versions?.OrderByDescending(x => x.CreatedUtc).ToHashSet().First().FileChecksum !=
                currentLocalFilesChecksum)
            {
                changedFiles.Add(trackedFile);
                continue;
            }
        }
        
        List<File> allScannedFiles = newFiles.Concat(untouchedFiles).Concat(changedFiles).ToList();
        List<File> deletedFiles = repo.FileRepo.QueryAll().Where(x => !allScannedFiles.Contains(x)).ToList();

        return new()
        {
            FilesChanged = changedFiles,
            FilesCreated = newFiles,
            FilesDeleted = deletedFiles,
            FilesUntouched = untouchedFiles,
        };
    }

    private async Task<Library> CreateLibraryLocal(Guid remoteId, string path)
    {
        Library newLibrary = new()
        {
            RemoteId = remoteId,
            Path = path,
        };
        
        await repo.LibraryRepo.InsertAsync(newLibrary);
        Directory.CreateDirectory(path);

        return newLibrary;
    }
}