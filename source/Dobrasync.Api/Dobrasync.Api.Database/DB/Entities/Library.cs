using Dobrasync.Api.Database.Entities.Base;

namespace Dobrasync.Api.Database.Entities;

public class Library : BaseEntity
{
    public required string Name { get; set; } = string.Empty;
    
    #region Relation
    public virtual HashSet<File> Files { get; set; } = new();
    #endregion
}