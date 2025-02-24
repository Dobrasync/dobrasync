using Dobrasync.Common.Clients.Database.DB.Entities.Base;

namespace Dobrasync.Common.Clients.Database.DB.Entities;

public class Library : BaseEntity
{
    public Guid RemoteId { get; set; }
    public required string Path { get; set; }
    public HashSet<File> Files { get; set; }
}