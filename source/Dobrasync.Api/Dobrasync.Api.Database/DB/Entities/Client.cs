using Dobrasync.Api.Database.Entities.Base;

namespace Dobrasync.Api.Database.Entities;

public class Client : BaseEntity
{
    public required string Name { get; set; }
    public Guid UserId { get; set; }
    public virtual required User User { get; set; }
}