using Dobrasync.Api.BusinessLogic.Dtos;
using Dobrasync.Api.Database.Entities;
using File = Dobrasync.Api.Database.Entities.File;

namespace Dobrasync.Api.BusinessLogic.Services.Main.Files;

public interface IFileService
{
    public Task<File> GetFileByIdAsync(Guid fileId);
    public Task<FileDto> GetFileByIdMappedAsync(Guid fileId);
    public Task<File> DeleteFileAsync(Guid fileId);
    public Task<FileDto> DeleteFileMappedAsync(Guid fileId);
    public Task<File> WriteFileAsync(Guid libraryId, string path, List<string> blockChecksums);
}