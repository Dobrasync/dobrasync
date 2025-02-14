namespace Dobrasync.Common.Clients.Database.DB.Entities.Base;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}