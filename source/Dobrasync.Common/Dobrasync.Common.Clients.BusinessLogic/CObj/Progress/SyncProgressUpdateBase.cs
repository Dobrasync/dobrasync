namespace Dobrasync.Common.Clients.BusinessLogic.CObj;

public abstract class SyncProgressUpdateBase
{
    public bool IsFatal { get; set; }
    public string Message { get; set; }

    public SyncProgressUpdateBase(string message, bool isFatal)
    {
        Message = message;
        IsFatal = isFatal;
    }
}