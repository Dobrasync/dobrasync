using Dobrasync.Api.Database.Entities.Base;

namespace Dobrasync.Api.Database.Entities;

public class OrderedBlock : BaseEntity
{
    public required int OrderIndex { get; set; }
    
    #region Relation
    public Guid VersionId { get; set; }
    public Version Version { get; set; } = null!;

    public Guid BlockId { get; set; }
    public Block Block { get; set; } = null!;
    #endregion
}