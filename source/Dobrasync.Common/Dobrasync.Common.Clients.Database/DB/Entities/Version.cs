namespace Dobrasync.Common.Clients.Database.DB.Entities;

public class Version
{
    public Guid RemoteId { get; set; }
    public DateTimeOffset CreatedUtc { get; set; }
    public required string FileChecksum { get; set; }
    public bool IsDirectoy { get; set; }
}