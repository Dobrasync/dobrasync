using Dobrasync.Api.Database.Entities.Base;
using Dobrasync.Api.Shared.Enums;

namespace Dobrasync.Api.Database.Entities;

public class Version : BaseEntity
{
    public DateTimeOffset CreatedUtc { get; set; }
    public EVersionStatus Status { get; set; }
    public List<byte[]> ExpectedBlocks { get; set; } = new();
    public List<Block> Blocks { get; set; } = new();
    #region File Metadata
    public DateTimeOffset FileCreatedOnUtc { get; set; }
    public DateTimeOffset FileModifiedOnUtc { get; set; }
    public byte[] FileChecksum { get; set; } = [];
    public int FilePermissionsOctal { get; set; }
    public bool IsDirectory { get; set; }
    #endregion
}