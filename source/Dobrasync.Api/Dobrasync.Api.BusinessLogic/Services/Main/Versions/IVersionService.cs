using Dobrasync.Api.BusinessLogic.Dtos.Versions;
using Dobrasync.Api.Database.Entities;
using File = Dobrasync.Api.Database.Entities.File;
using Version = Dobrasync.Api.Database.Entities.Version;

namespace Dobrasync.Api.BusinessLogic.Services.Main.Transactions;

public interface IVersionService
{
    public Task<Version> CreateAsync(VersionCreateDto createDto);
    public Task<Version> CompleteAsync(Guid transactionId);
    public Task<Version> DeleteVersionAsync(Guid versionId);
    public Task<bool> IsFileLockedAsync(Guid file);
}