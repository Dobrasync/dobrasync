using Dobrasync.Api.Database.Entities.Base;

namespace Dobrasync.Api.Database.Entities;

public class Client : BaseEntity
{
    public required string Name { get; set; }
    
    #region Relation
    public virtual required User User { get; set; }
    public Guid UserId { get; set; }
    #endregion
}