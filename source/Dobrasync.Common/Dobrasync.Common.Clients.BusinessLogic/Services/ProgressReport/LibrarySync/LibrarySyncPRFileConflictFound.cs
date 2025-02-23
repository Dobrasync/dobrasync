using Dobrasync.Common.Clients.BusinessLogic.CObj.Etc;

namespace Dobrasync.Common.Clients.BusinessLogic.CObj;

public class LibrarySyncPRFileConflictFound : LibrarySyncPR
{
    public FileConflictDescription FileConflictDescription { get; set; }
    
    public LibrarySyncPRFileConflictFound(FileConflictDescription desc) : base("A file conflict was found.", false)
    {
        FileConflictDescription = desc;
    }
}