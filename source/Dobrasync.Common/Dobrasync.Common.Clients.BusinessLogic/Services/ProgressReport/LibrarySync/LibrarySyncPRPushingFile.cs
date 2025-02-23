using Dobrasync.Common.Clients.BusinessLogic.CObj.Etc;

namespace Dobrasync.Common.Clients.BusinessLogic.CObj;

public class LibrarySyncPRPushingFile : LibrarySyncPR
{
    
    public LibrarySyncPRPushingFile(string path) : base($"Pushing file '{path}'.", false)
    {
        
    }
}