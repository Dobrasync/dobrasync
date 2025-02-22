using System;
using System.Threading.Tasks;
using Dobrasync.Common.Clients.Api;
using Dobrasync.Common.Clients.BusinessLogic.CObj;
using Dobrasync.Common.Clients.BusinessLogic.CObj.Progress.LibDelete;

namespace Dobrasync.Common.Clients.BusinessLogic.Services.Main.Libraries;

public interface ILibraryService
{
    /// <summary>
    /// Fetches a remote library and creates a local entry, as well as a library
    /// directory. Does not automatically synchronize or download any files.
    /// </summary>
    /// <param name="remoteId"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public Task CloneLibraryAsync(Guid remoteId, string path);
    
    /// <summary>
    /// Removes a library from local and removes the locally stored files. Remote
    /// is unaffected.
    /// </summary>
    /// <param name="localLibraryId"></param>
    /// <returns></returns>
    public Task UncloneLibraryAsync(Guid localLibraryId);
    
    /// <summary>
    /// Deletes a library from remote. Local files are unaffected.
    /// </summary>
    /// <param name="remoteId"></param>
    /// <returns></returns>
    public Task<DeleteLibraryResult> DeleteLibraryAsync(Guid remoteId, IProgress<LibDeletePRBase> progress, CancellationToken cancellationToken);
    
    /// <summary>
    /// Creates a new library on remote.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<LibraryDto> CreateLibraryAsync(string name);
    
    /// <summary>
    /// Synchronizes the specified libraries files.
    /// </summary>
    /// <param name="progress"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="libraryId"></param>
    /// <returns></returns>
    public Task<SyncResult> SyncLibraryAsync(Guid libraryId, IProgress<SyncProgressUpdateBase> progress, CancellationToken cancellationToken);
}