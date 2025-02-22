using Dobrasync.Common.Clients.BusinessLogic.CObj.Etc;

namespace Dobrasync.Common.Clients.BusinessLogic.CObj;

public class SyncPUPushingFile : SyncProgressUpdateBase
{
    
    public SyncPUPushingFile(string path) : base($"Pushing file '{path}'.", false)
    {
        
    }
}