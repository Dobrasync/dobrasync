using AutoMapper;
using Dobrasync.Api.BusinessLogic.Dtos;
using Dobrasync.Api.BusinessLogic.Dtos.Diff;
using Dobrasync.Api.BusinessLogic.Services.Core.AccessControl;
using Dobrasync.Api.BusinessLogic.Services.Core.AppsettingsProvider;
using Dobrasync.Api.BusinessLogic.Services.Main.Files;
using Dobrasync.Api.BusinessLogic.Services.Main.Transactions;
using Dobrasync.Api.Database.Entities;
using Dobrasync.Api.Database.Repos;
using Dobrasync.Api.Shared.Enums;
using Dobrasync.Api.Shared.Exceptions.Userspace;
using Dobrasync.Api.Shared.Util;
using Dobrasync.Common.Util;
using Microsoft.EntityFrameworkCore;
using File = Dobrasync.Api.Database.Entities.File;
using Version = Dobrasync.Api.Database.Entities.Version;

namespace Dobrasync.Api.BusinessLogic.Services.Main.Libraries;

public class LibraryService(IRepoWrapper repo, IMapper mapper, IAccessControlService acs, IFileService fileService, IAppsettingsProviderService apps) : ILibraryService
{
    public async Task<Library> GetLibraryByNameAsync(string name)
    {
        #region Load
        Library? library = await repo.LibraryRepo.QueryAll().FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        if (library == null) throw new NotFoundUSException();
        #endregion
        
        return library;
    }

    public async Task<LibraryDto> GetLibraryByNameMappedAsync(string name)
    {
        Library lib = await GetLibraryByNameAsync(name);
        
        return mapper.Map<LibraryDto>(lib);
    }

    public async Task<Library> GetLibraryByIdAsync(Guid id)
    {
        #region Load
        Library? library = await repo.LibraryRepo.QueryAll().FirstOrDefaultAsync(x => x.Id == id);
        if (library == null) throw new NotFoundUSException();
        #endregion
        
        return library;
    }

    public async Task<LibraryDto> GetLibraryByIdMappedAsync(Guid id)
    {
        Library lib = await GetLibraryByIdAsync(id);
        
        return mapper.Map<LibraryDto>(lib);
    }
    
    public async Task<Library> CreateLibraryAsync(string name)
    {
        Library? existing = await repo.LibraryRepo.QueryAll().FirstOrDefaultAsync(x => x.Name == name);
        if (existing != null)
        {
            throw new LibraryNameConflictUSException();
        }
        
        Library newLibrary = new()
        {
            Name = name
        };
        Library created = await repo.LibraryRepo.InsertAsync(newLibrary);
        
        Directory.CreateDirectory(Pathing.GetPathToLibraryData(apps.GetAppsettings(), created.Id));
        
        return created;
    }

    public async Task<LibraryDto> CreateLibraryMappedAsync(string name)
    {
        Library created = await CreateLibraryAsync(name);
        
        return mapper.Map<LibraryDto>(created);
    }

    public async Task<Library> DeleteLibraryAsync(Guid librayId)
    {
        Library? existing = await repo.LibraryRepo
            .QueryAll()
            .Include(x => x.Files)
            .FirstOrDefaultAsync(x => x.Id == librayId);
        if (existing == null)
        {
            throw new NotFoundUSException();
        }

        User invoker = await acs.VerifyAsync(opt =>
        {
            opt.RequireLibraryOwnership = existing;
        });

        foreach (var file in existing.Files)
        {
            await fileService.DeleteFileAsync(file.Id);
        }

        return existing;
    }

    public async Task<LibraryDto> DeleteLibraryMappedAsync(Guid librayId)
    {
        Library deleted = await DeleteLibraryAsync(librayId);

        return mapper.Map<LibraryDto>(deleted);
    }

    public async Task<File> GetLibraryFileAsync(Guid libraryId, string path)
    {
        #region Load
        File? file = await repo.FileRepo.QueryAll()
            .Include(x => x.Library)
            .FirstOrDefaultAsync(x => x.Library.Id == libraryId && x.Path == path);
        if (file == null) throw new NotFoundUSException();
        #endregion

        return file;
    }

    public async Task<FileDto> GetLibraryFileMappedAsync(Guid libraryId, string path)
    {
        File file = await GetLibraryFileAsync(libraryId, path);

        return mapper.Map<FileDto>(file);
    }

    public async Task<List<string>> MakeDiffMappedAsync(Guid libraryId, DiffCreateDto diffCreateDto)
    {
        #region Load
        Library? library = await repo.LibraryRepo
            .QueryAll()
            .Include(x => x.Files)
            .ThenInclude(x => x.Versions)
            .FirstOrDefaultAsync(x => x.Id == libraryId);
        
        if (library == null) throw new NotFoundUSException();
        #endregion
        
        List<string> allFilePaths = library.Files.Select(x => x.Path).ToList();
        allFilePaths.AddRange(diffCreateDto.FilesOnLocal.Select(x => x.Path).Except(allFilePaths));
        
        List<string> diff = [];
        foreach (string path in allFilePaths)
        {
            File? fileOnRemote = library.Files.FirstOrDefault(x => x.Path == path);
            if (fileOnRemote == null)
            {
                diff.Add(path);
                continue;
            }
         
            DiffFileDescriptionDto? fileOnLocal = diffCreateDto.FilesOnLocal.FirstOrDefault(x => x.Path == path);
            Version? latestVersionOnRemote = repo.VersionRepo.QueryAll()
                .OrderByDescending(x => x.CreatedUtc)
                .FirstOrDefault();

            // Server doesn't have a version for this file, client needs to push
            if (latestVersionOnRemote == null || fileOnLocal == null)
            {
                diff.Add(path);
                continue;
            }

            // Client doesnt know about latest file version on server
            if (latestVersionOnRemote.Id != fileOnLocal.LatestVersionId)
            {
                diff.Add(path);
                continue;
            }
            
            // if versions match on both client and server, check if checksums are different
            // if checksums differ it means the client want to create a new version
            if (fileOnLocal.FileChecksum != latestVersionOnRemote.FileChecksum)
            {
                diff.Add(path);
                continue;
            }
        }

        return diff;
    }
}