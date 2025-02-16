namespace Dobrasync.Common.Clients.Database.DB.Entities;

public class File
{
    public HashSet<Version> Versions { get; set; } = new();
    public required string Path { get; set; }
    public Library Library { get; set; } = null!;
    public Guid LibraryId { get; set; }
}