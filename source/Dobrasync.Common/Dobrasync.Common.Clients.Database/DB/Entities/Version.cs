using Dobrasync.Common.Clients.Database.DB.Entities.Base;

namespace Dobrasync.Common.Clients.Database.DB.Entities;

public class Version : BaseEntity
{
    public Guid RemoteId { get; set; }
    public File File { get; set; }
    public Guid FileId { get; set; }
    public DateTimeOffset CreatedUtc { get; set; }
    public required string FileChecksum { get; set; }
    public bool IsDirectoy { get; set; }
}