using Dobrasync.Api.Database.Entities.Base;

namespace Dobrasync.Api.Database.Entities;

public class Block : BaseEntity
{
    public required string Checksum { get; set; } = string.Empty;
    
    #region Relation
    public Library Library { get; set; } = null!;
    public Guid LibraryId { get; set; }
    public HashSet<Version> Versions { get; set; } = new();
    #endregion
}