using System.Collections.Generic;

namespace Dobrasync.Common.Clients.BusinessLogic.CObj;

public class SyncResult
{
    public bool Fatal { get; set; } = false;
    public HashSet<string> PushedFiles { get; set; } = new();
    public HashSet<string> PulledFiles { get; set; } = new();
    public HashSet<string> UndecidedFiles { get; set; } = new();
    public HashSet<string> FailedFiles { get; set; } = new();
}