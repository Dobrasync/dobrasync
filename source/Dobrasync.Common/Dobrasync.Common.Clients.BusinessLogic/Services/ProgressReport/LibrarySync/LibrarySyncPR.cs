using Dobrasync.Common.Clients.BusinessLogic.Services.ProgressReport;

namespace Dobrasync.Common.Clients.BusinessLogic.CObj;

public abstract class LibrarySyncPR : ServiceProgressReport
{
    public LibrarySyncPR(string message, bool isFatal) : base(message)
    {
    }
}