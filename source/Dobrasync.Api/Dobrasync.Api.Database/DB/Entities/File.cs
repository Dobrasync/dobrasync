using Dobrasync.Api.Database.Entities.Base;

namespace Dobrasync.Api.Database.Entities;

public class File : BaseEntity
{
    public required string Path { get; set; }
    
    #region Relation
    public Library Library { get; set; } = null!;
    public Guid LibraryId { get; set; }
    public HashSet<Version> Versions { get; set; } = new();
    #endregion
}