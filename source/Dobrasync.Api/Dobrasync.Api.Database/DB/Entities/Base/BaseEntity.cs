namespace Dobrasync.Api.Database.Entities.Base;

public class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}