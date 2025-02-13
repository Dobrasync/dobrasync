namespace Dobrasync.Api.BusinessLogic.Dtos.Versions;

public class VersionCreateDto
{
    public Guid LibraryId { get; set; }
    public required string FilePath { get; set; }
    public DateTimeOffset FileCreatedOnUtc { get; set; }
    public DateTimeOffset FileModifiedOnUtc { get; set; }
    public required string FileChecksum { get; set; }
    public List<string> ExpectedBlocks { get; set; } = [];
    public int FilePermissionsOctal { get; set; }
    public bool IsDirectory { get; set; }
}