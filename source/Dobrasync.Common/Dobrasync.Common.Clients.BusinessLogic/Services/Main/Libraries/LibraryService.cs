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
    public async Task CloneLibrary(Guid remoteId, string path)
    {
        if (repo.LibraryRepo.QueryAll().Any(x => x.RemoteId == remoteId))
        {
            return;
        }
        
        LibraryDto remoteLibrary = await api.GetLibraryByIdAsync(remoteId);
        Library created = await CreateLibraryLocal(remoteLibrary.Id, path);
    }

    public async Task SyncLibrary(Guid id)
    {
        Library? library = await repo.LibraryRepo
            .QueryAll()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (library == null) return;
        
        List<FileInfo> filesWithinLib = await CreateAndUpdateLocalLibraryFileTree(library.Id);
        
    }

    private async Task<List<FileInfo>> CreateAndUpdateLocalLibraryFileTree(Guid libraryId)
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
        
        return files;
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

        return newLibrary;
    }
}