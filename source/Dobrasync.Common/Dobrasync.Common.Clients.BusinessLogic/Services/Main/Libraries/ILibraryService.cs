namespace Dobrasync.Common.Clients.BusinessLogic.Services.Main.Libraries;

public interface ILibraryService
{
    public Task CloneLibrary(Guid remoteId, string path);
    public Task SyncLibrary(Guid id);
}