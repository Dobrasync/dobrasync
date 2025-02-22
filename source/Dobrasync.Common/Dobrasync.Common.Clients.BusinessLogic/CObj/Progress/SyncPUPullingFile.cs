using Dobrasync.Common.Clients.BusinessLogic.CObj.Etc;

namespace Dobrasync.Common.Clients.BusinessLogic.CObj;

public class SyncPUPullingFile : SyncProgressUpdateBase
{
    
    public SyncPUPullingFile(string path) : base($"Pulling file '{path}'.", false)
    {
        
    }
}