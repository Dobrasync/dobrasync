namespace Dobrasync.Api.BusinessLogic.Dtos.Versions;

public class VersionCreateResultDto
{
    public required List<string> RequiredBlocks { get; set; } = new();
    public required VersionDto CreatedVersion { get; set; }
}