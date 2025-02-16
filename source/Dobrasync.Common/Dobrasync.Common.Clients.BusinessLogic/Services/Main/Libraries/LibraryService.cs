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

    public async Task SyncLibraryAsync(Guid id)
    {
        Library? library = await repo.LibraryRepo
            .QueryAll()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (library == null) return;
        
        FileTreeBuildResult tree = await CreateAndUpdateLocalLibraryFileTree(library.Id);
        
        #region Get diff
        ICollection<string> diff = await api.MakeLibraryDiffAsync(id, new()
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
        
        #region Pull/Push list
        HashSet<string> filesToPush = new();
        HashSet<string> filesToPull = new();
        HashSet<string> filesUndecided = new();
        HashSet<string> filesFailure = new();
        
        foreach (string filepath in diff)
        {
            File? localFile = await repo.FileRepo
                .QueryAll()
                .Where(x => x.LibraryId == id)
                .Include(x => x.Versions)
                .FirstOrDefaultAsync(x => x.Path == filepath);

            #region Pull - If local doesnt have match
            if (localFile == null)
            {
                filesToPull.Add(filepath);
                continue;
            }
            #endregion
            #region Push - if remote doesnt have match
            FileDto? apiFileInfo = null;
            try
            {
                apiFileInfo = await api.GetLibraryFileAsync(id, filepath);
            }
            catch (ApiException apiEx)
            {
                if (apiEx.StatusCode == 404)
                {
                    filesToPush.Add(filepath);
                    continue;
                }
            }
            #endregion
            #region Fail - if remote returned unknown error
            if (apiFileInfo == null)
            {
                filesFailure.Add(filepath);
                continue;
            }
            #endregion

            VersionDto latestFileVersion = await api.GetVersionRequiredAsync(apiFileInfo.CurrentVersionId);
            #region Pull - if local has no versions
            if (localFile.Versions.Count == 0)
            {
                filesToPull.Add(filepath);
                continue;
            }
            #endregion
            
            // in case of conflict, let user decide if the local version or remote version should be used
            if (latestFileVersion.Id != localFile.Versions.OrderByDescending(x => x.CreatedUtc).First().RemoteId)
            {
                filesToPush.Add(filepath);
                continue;
            }

            throw new ArgumentException("Unexpected condition");
        }
        #endregion
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