namespace Dobrasync.Common.Clients.BusinessLogic.CObj;

public class FileWithVersionInfo
{
    public required string Path { get; set; }
    public Guid? LatestVersion { get; set; }
}