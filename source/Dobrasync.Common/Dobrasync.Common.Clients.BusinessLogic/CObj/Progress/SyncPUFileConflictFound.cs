using Dobrasync.Common.Clients.BusinessLogic.CObj.Etc;

namespace Dobrasync.Common.Clients.BusinessLogic.CObj;

public class SyncPUFileConflictFound : SyncProgressUpdateBase
{
    public FileConflictDescription FileConflictDescription { get; set; }
    
    public SyncPUFileConflictFound(FileConflictDescription desc) : base("A file conflict was found.", false)
    {
        FileConflictDescription = desc;
    }
}