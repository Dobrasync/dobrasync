using Dobrasync.Api.BusinessLogic.Dtos;
using Dobrasync.Api.BusinessLogic.Dtos.Diff;
using Dobrasync.Api.Database.Entities;
using File = Dobrasync.Api.Database.Entities.File;

namespace Dobrasync.Api.BusinessLogic.Services.Main.Libraries;

public interface ILibraryService
{
    public Task<Library> GetLibraryByNameAsync(string name);
    public Task<LibraryDto> GetLibraryByNameMappedAsync(string name);
    public Task<Library> GetLibraryByIdAsync(Guid id);
    public Task<LibraryDto> GetLibraryByIdMappedAsync(Guid id);
    public Task<Library> CreateLibraryAsync(string name);
    public Task<LibraryDto> CreateLibraryMappedAsync(string name);
    public Task<Library> DeleteLibraryAsync(Guid libraryId);
    public Task<LibraryDto> DeleteLibraryMappedAsync(Guid libraryId);
    public Task<File> GetLibraryFileAsync(Guid libraryId, string path);
    public Task<FileDto> GetLibraryFileMappedAsync(Guid libraryId, string path);
    public Task<List<string>> MakeDiffMappedAsync(Guid libraryId, DiffCreateDto diffCreateDto);
}