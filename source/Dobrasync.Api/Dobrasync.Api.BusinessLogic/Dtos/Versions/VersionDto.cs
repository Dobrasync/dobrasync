using Dobrasync.Api.Shared.Enums;

namespace Dobrasync.Api.BusinessLogic.Dtos.Versions;

public class VersionDto
{
    public Guid Id { get; set; }
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
}