namespace Dobrasync.Common.Clients.BusinessLogic.CObj.Etc;

public class FileConflictDescription
{
    public required string Path { get; set; }
    public required DateTimeOffset LocalFileLastModifiedUtc { get; set; }
    public required DateTimeOffset RemoteFileLastModifiedUtc { get; set; }
}