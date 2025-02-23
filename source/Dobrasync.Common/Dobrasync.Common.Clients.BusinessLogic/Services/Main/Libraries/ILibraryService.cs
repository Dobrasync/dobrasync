using System;
using System.Threading.Tasks;
using Dobrasync.Common.Clients.Api;
using Dobrasync.Common.Clients.BusinessLogic.CObj;
using Dobrasync.Common.Clients.BusinessLogic.CObj.Progress.LibDelete;
using Dobrasync.Common.Clients.BusinessLogic.Services.ActionResults;
using Dobrasync.Common.Clients.BusinessLogic.Services.ProgressReport.LibraryClone;
using Dobrasync.Common.Clients.BusinessLogic.Services.ProgressReport.LibraryCreate;
using Dobrasync.Common.Clients.BusinessLogic.Services.ProgressReport.LibraryDelete;
using Dobrasync.Common.Clients.BusinessLogic.Services.ProgressReport.LibraryUnclone;

namespace Dobrasync.Common.Clients.BusinessLogic.Services.Main.Libraries;

public interface ILibraryService
{
    /// <summary>
    /// Fetches a remote library and creates a local entry, as well as a library
    /// directory. Does not automatically synchronize or download any files.
    /// </summary>
    /// <param name="remoteId"></param>
    /// <param name="path"></param>
    /// <param name="progress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<LibraryCloneSAR> CloneLibraryAsync(Guid remoteId, string path, IProgress<LibraryClonePR> progress, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a library from local and removes the locally stored files. Remote
    /// is unaffected.
    /// </summary>
    /// <param name="localLibraryId"></param>
    /// <param name="progress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<LibraryUncloneSAR> UncloneLibraryAsync(Guid localLibraryId, IProgress<LibraryUnclonePR> progress, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a library from remote. Local files are unaffected.
    /// </summary>
    /// <param name="remoteId"></param>
    /// <param name="progress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<LibraryDeleteSAR> DeleteLibraryAsync(Guid remoteId, IProgress<LibraryDeletePR> progress, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new library on remote.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="progress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<LibraryCreateSAR> CreateLibraryAsync(string name, IProgress<LibraryCreatePR> progress, CancellationToken cancellationToken);
    
    /// <summary>
    /// Synchronizes the specified libraries files.
    /// </summary>
    /// <param name="progress"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="libraryId"></param>
    /// <returns></returns>
    public Task<LibrarySyncSAR> SyncLibraryAsync(Guid libraryId, IProgress<LibrarySyncPR> progress, CancellationToken cancellationToken);
}