using Dobrasync.Api.Database.Entities.Base;
using Dobrasync.Api.Shared.Enums;

namespace Dobrasync.Api.Database.Entities;

public class Version : BaseEntity
{
    #region Version Data
    public DateTimeOffset CreatedUtc { get; set; }
    public EVersionStatus Status { get; set; }
    public List<string> ExpectedBlocks { get; set; } = new();
    #endregion
    #region File Metadata
    public DateTimeOffset FileCreatedOnUtc { get; set; }
    public DateTimeOffset FileModifiedOnUtc { get; set; }
    public required string FileChecksum { get; set; } = string.Empty;
    public int FilePermissionsOctal { get; set; }
    public bool IsDirectory { get; set; }
    #endregion
    #region Relation
    public List<OrderedBlock> OrderedBlocks { get; set; } = new();
    public File File { get; set; } = null!;
    public Guid FileId { get; set; }
    #endregion
}