using Dobrasync.Common.Clients.BusinessLogic.CObj.Etc;

namespace Dobrasync.Common.Clients.BusinessLogic.CObj;

public class SyncPUComplete : SyncProgressUpdateBase
{
    
    public SyncPUComplete() : base($"Sync complete.", false)
    {
        
    }
}