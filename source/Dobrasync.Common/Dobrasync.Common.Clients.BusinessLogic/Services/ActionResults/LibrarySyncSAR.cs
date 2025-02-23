namespace Dobrasync.Common.Clients.BusinessLogic.Services.ActionResults;

public class LibrarySyncSAR : ServiceActionResult
{
    public HashSet<string> PushedFiles { get; set; } = new();
    public HashSet<string> FailedFiles { get; set; } = new();
    public HashSet<string> PulledFiles { get; set; } = new();
    public HashSet<string> UndecidedFiles { get; set; } = new();
}