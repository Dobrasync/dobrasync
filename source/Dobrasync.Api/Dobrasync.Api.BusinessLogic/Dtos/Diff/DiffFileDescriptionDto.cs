namespace Dobrasync.Api.BusinessLogic.Dtos.Diff;

public class DiffFileDescriptionDto
{
    public required string Path { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset ModifiedOnUtc { get; set; }
    public int PermissionsOctal { get; set; }
    public required byte[] Checksum { get; set; }
    public List<byte[]> Blocks { get; set; } = new();
    public Guid? LastTransaction { get; set; }
}