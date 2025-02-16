using System;
using System.Threading.Tasks;
using Dobrasync.Common.Clients.Api;

namespace Dobrasync.Common.Clients.BusinessLogic.Services.Main.Libraries;

public interface ILibraryService
{
    public Task CloneLibraryAsync(Guid remoteId, string path);
    public Task<LibraryDto> CreateLibraryAsync(string name);
    public Task SyncLibraryAsync(Guid id);
}