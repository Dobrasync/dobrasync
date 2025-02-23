using Dobrasync.Common.Clients.BusinessLogic.CObj.Etc;

namespace Dobrasync.Common.Clients.BusinessLogic.CObj;

public class LibrarySyncPRPullingFile : LibrarySyncPR
{
    
    public LibrarySyncPRPullingFile(string path) : base($"Pulling file '{path}'.", false)
    {
        
    }
}