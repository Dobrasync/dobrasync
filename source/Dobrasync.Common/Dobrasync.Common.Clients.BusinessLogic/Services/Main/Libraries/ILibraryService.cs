using System;
using System.Threading.Tasks;
using Dobrasync.Common.Clients.Api;
using Dobrasync.Common.Clients.BusinessLogic.CObj;

namespace Dobrasync.Common.Clients.BusinessLogic.Services.Main.Libraries;

public interface ILibraryService
{
    public Task CloneLibraryAsync(Guid remoteId, string path);
    public Task<LibraryDto> CreateLibraryAsync(string name);
    public Task SyncLibraryAsync(IProgress<SyncProgressUpdateBase> progress, CancellationToken cancellationToken, Guid libraryId);
}