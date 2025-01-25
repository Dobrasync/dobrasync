namespace Dobrasync.Common.Clients.Database.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}