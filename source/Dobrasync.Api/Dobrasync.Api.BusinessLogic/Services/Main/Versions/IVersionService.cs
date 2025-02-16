using Dobrasync.Api.BusinessLogic.Dtos.Versions;
using Dobrasync.Api.Database.Entities;
using File = Dobrasync.Api.Database.Entities.File;
using Version = Dobrasync.Api.Database.Entities.Version;

namespace Dobrasync.Api.BusinessLogic.Services.Main.Transactions;

public interface IVersionService
{
    public Task<Version> GetVersionRequiredAsync(Guid id);
    public Task<VersionDto> GetVersionRequiredMappedAsync(Guid id);
    public Task<VersionCreateResultDto> CreateVersionAsync(VersionCreateDto createDto);
    public Task<VersionCreateResultDto> CreateVersionMappedAsync(VersionCreateDto createDto);
    public Task<Version> CompleteAsync(Guid transactionId);
    public Task<VersionDto> CompleteMappedAsync(Guid transactionId);
    public Task<Version> DeleteVersionAsync(Guid versionId);
    public Task<List<string>> GetVersionBlocksAsync(Guid id);
    public Task<List<Version>> GetFileVersionsNewerThanDateAsync(Guid fileId, DateTimeOffset dateUtc);
    public Task<List<VersionDto>> GetFileVersionsNewerThanDateMappedAsync(Guid fileId, DateTimeOffset dateUtc);
}