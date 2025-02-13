using Dobrasync.Api.Database.Entities.Base;
using Dobrasync.Api.Shared.Enums;

namespace Dobrasync.Api.Database.Entities;

public class Version : BaseEntity
{
    public DateTimeOffset CreatedUtc { get; set; }
    public EVersionStatus Status { get; set; }
    public List<string> ExpectedBlocks { get; set; } = new();
    public List<Block> Blocks { get; set; } = new();
    public File File { get; set; } = null!;
    #region File Metadata
    public DateTimeOffset FileCreatedOnUtc { get; set; }
    public DateTimeOffset FileModifiedOnUtc { get; set; }
    public required string FileChecksum { get; set; } = string.Empty;
    public int FilePermissionsOctal { get; set; }
    public bool IsDirectory { get; set; }
    #endregion
}