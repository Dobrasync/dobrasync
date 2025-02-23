namespace Dobrasync.Common.Clients.BusinessLogic.Services.ProgressReport;

public abstract class ServiceProgressReport
{
    public string Message { get; set; }

    public ServiceProgressReport(string message)
    {
        Message = message;
    }
}