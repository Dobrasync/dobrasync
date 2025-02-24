using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dobrasync.Common.Clients.Api;
using Dobrasync.Common.Clients.BusinessLogic.CObj;
using Dobrasync.Common.Clients.BusinessLogic.CObj.Progress.LibCreate;
using Dobrasync.Common.Clients.BusinessLogic.CObj.Progress.LibDelete;
using Dobrasync.Common.Clients.BusinessLogic.Services.ActionResults;
using Dobrasync.Common.Clients.BusinessLogic.Services.ProgressReport.LibraryClone;
using Dobrasync.Common.Clients.BusinessLogic.Services.ProgressReport.LibraryCreate;
using Dobrasync.Common.Clients.BusinessLogic.Services.ProgressReport.LibraryDelete;
using Dobrasync.Common.Clients.BusinessLogic.Services.ProgressReport.LibraryUnclone;
using Dobrasync.Common.Clients.Database.DB.Entities;
using Dobrasync.Common.Clients.Database.Repos;
using Dobrasync.Common.Clients.Shared.Exceptions;
using Dobrasync.Common.Util;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using File = Dobrasync.Common.Clients.Database.DB.Entities.File;
using Version = Dobrasync.Common.Clients.Database.DB.Entities.Version;

namespace Dobrasync.Common.Clients.BusinessLogic.Services.Main.Libraries;

public class LibraryService(IApiClient api, IRepoWrapper repo) : ILibraryService
{
    public async Task<LibraryCloneSAR> CloneLibraryAsync(Guid remoteId, string path, IProgress<LibraryClonePR> progress, CancellationToken cancellationToken)
    {
        if (repo.LibraryRepo.QueryAll().Any(x => x.RemoteId == remoteId))
        {
            return new()
            {
                
            };
        }
        
        LibraryDto remoteLibrary = await api.GetLibraryByIdAsync(remoteId);
        LibCreateResult created = await CreateLibraryLocal(remoteLibrary.Id, Path.Join(path, remoteLibrary.Name));

        return new()
        {

        };
    }

    public async Task<LibraryCreateSAR> CreateLibraryAsync(string name, IProgress<LibraryCreatePR> progress, CancellationToken cancellationToken)
    {
        LibraryDto created = await api.CreateLibraryAsync(new()
        {
            Name = name,
        });
        
        return new()
        {
            CreatedLibrary = created,
        };
    }

    public async Task<LibrarySyncSAR> SyncLibraryAsync(Guid libraryId, IProgress<LibrarySyncPR> progress, CancellationToken cancellationToken)
    {
        #region Load library
        Library? library = await repo.LibraryRepo
            .QueryAll()
            .FirstOrDefaultAsync(x => x.Id == libraryId);

        if (library == null)
        {
            progress.Report(new LibrarySyncPRFatalLibraryNotFound());
            return new()
            {
                
            };
        }
        #endregion
        #region Build file tree
        progress.Report(new LibrarySyncPRBuildingFileTree());
        FileTreeBuildResult tree = await CreateAndUpdateLocalLibraryFileTree(library.Id);
        #endregion
        #region Get diff
        progress.Report(new LibrarySyncPRFetchingDiff());

        List<DiffFileDescriptionDto> fileDescs = new();
        foreach (var file in tree.FilesAll)
        {
            string fileSysPath = Path.Join(library.Path, file.Path);
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(fileSysPath);
            string fileCheck = ChecksumUtil.CalculateFileChecksum(fileBytes);
            Guid? latestVersionId = file.Versions.OrderByDescending(v => v.CreatedUtc).ToList().FirstOrDefault()?.RemoteId;
            
            DiffFileDescriptionDto newFileDesc = new()
            {
                Path = file.Path,
                FileChecksum = fileCheck,
                LatestVersionId = latestVersionId,
            };
            fileDescs.Add(newFileDesc);
        }
        
        ICollection<string> diff = await api.MakeLibraryDiffAsync(library.RemoteId, new()
        {
            FilesOnLocal = fileDescs,
        });
        #endregion
        
        #region Build Pull/Push list
        progress.Report(new LibrarySyncPRBuildingPushPullLists());
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
        
        HashSet<string> actuallyFilesPulled = new();
        HashSet<string> actuallyFilesPushed = new();
        HashSet<string> actuallyFilesFailed = new();
        
        #region Pull
        foreach (var file in filesToPull)
        {
            VersionDto remoteVersion = await api.GetVersionRequiredAsync(file.LatestVersion!.Value);
            progress.Report(new LibrarySyncPRPullingFile(file.Path));
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

            File fileDb = await repo.FileRepo.QueryAll()
                .Include(x => x.Versions)
                .Where(x => x.LibraryId == library.Id)
                .FirstAsync(x => x.Path == file.Path);

            fileDb.Versions.Add(new Version()
            {
                FileChecksum = remoteVersion.FileChecksum,
                RemoteId = remoteVersion.Id,
                CreatedUtc = remoteVersion.CreatedUtc,
                IsDirectoy = remoteVersion.IsDirectory,
            });
            
            actuallyFilesPulled.Add(file.Path);
        }
        #endregion
        #region Push
        foreach (var file in filesToPush)
        {
            progress.Report(new LibrarySyncPRPushingFile(file.Path));
            
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
                LibraryId = library.RemoteId,
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
            VersionDto completeVersion = await api.CompleteVersionAsync(newVersion.CreatedVersion.Id);
            #endregion
            
            File fileDb = await repo.FileRepo.QueryAll()
                .Include(x => x.Versions)
                .Where(x => x.LibraryId == library.Id)
                .FirstAsync(x => x.Path == file.Path);

            fileDb.Versions.Add(new Version()
            {
                FileChecksum = completeVersion.FileChecksum,
                RemoteId = completeVersion.Id,
                CreatedUtc = completeVersion.CreatedUtc,
                IsDirectoy = completeVersion.IsDirectory,
            });
            
            actuallyFilesPushed.Add(file.Path);
        };
        #endregion
        
        progress.Report(new LibrarySyncPRComplete());
        return new()
        {
            UndecidedFiles = filesUndecided.Select(x => x.Path).ToHashSet(),
            FailedFiles = actuallyFilesFailed,
            PulledFiles = actuallyFilesPulled,
            PushedFiles = actuallyFilesPushed,
        };
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
                .Where(x => x.LibraryId == library.Id)
                .Include(x => x.Versions)
                .FirstOrDefaultAsync(x => x.Path == filePathInLibrary);

            if (trackedFile == null)
            {
                trackedFile = new File()
                {
                    Path = filePathInLibrary,
                    LibraryId = library.Id,
                };
                await repo.FileRepo.InsertAsync(trackedFile);
                newFiles.Add(trackedFile);
                continue;
            }

            if (trackedFile.Versions?.Count == 0)
            {
                untouchedFiles.Add(trackedFile);
                continue;
            }
            
            byte[] fileBytesLocal = await System.IO.File.ReadAllBytesAsync(fileInfo.FullName);
            string currentLocalFilesChecksum = ChecksumUtil.CalculateFileChecksum(fileBytesLocal);
            if (trackedFile.Versions?.OrderByDescending(x => x.CreatedUtc).ToHashSet().First().FileChecksum !=
                currentLocalFilesChecksum)
            {
                changedFiles.Add(trackedFile);
                continue;
            }
            
            untouchedFiles.Add(trackedFile);
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

    private async Task<LibCreateResult> CreateLibraryLocal(Guid remoteId, string path)
    {
        Library newLibrary = new()
        {
            RemoteId = remoteId,
            Path = path,
        };
        
        await repo.LibraryRepo.InsertAsync(newLibrary);
        Directory.CreateDirectory(path);

        return new()
        {
            
        };
    }

    public async Task<LibraryDeleteSAR> DeleteLibraryAsync(Guid remoteId, IProgress<LibraryDeletePR> progress, CancellationToken cancellationToken)
    {
        #region load
        await api.DeleteLibraryByIdAsync(remoteId);
        #endregion

        return new()
        {
            //Deleted = true
        };
    }

    public async Task<LibraryUncloneSAR> UncloneLibraryAsync(Guid localLibraryId, IProgress<LibraryUnclonePR> progress,
        CancellationToken cancellationToken)
    {
        #region Load db entry
        Library? lib = await repo.LibraryRepo
            .QueryAll()
            .Include(x => x.Files)
            .ThenInclude(x => x.Versions)
            .FirstOrDefaultAsync(x => x.Id == localLibraryId);

        if (lib == null)
        {
            throw new NotFoundUSException();
        }
        #endregion
        #region remove library from database
        progress.Report(new LibraryUnclonePRRemovingDatabaseEntry());
        
        await repo.LibraryRepo.DeleteAsync(lib);
        #endregion
        #region remove files from file system
        progress.Report(new LibraryUnclonePRRemovingFiles());
        Directory.Delete(lib.Path, true);
        #endregion
        
        progress.Report(new LibraryUnclonePRComplete());
        return new()
        {

        };
    }
}