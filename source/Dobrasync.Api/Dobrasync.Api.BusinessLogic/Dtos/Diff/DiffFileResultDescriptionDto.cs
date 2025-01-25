using Dobrasync.Common.Util;

namespace Dobrasync.Api.BusinessLogic.Dtos.Diff;

public class DiffFileResultDescriptionDto
{
    public required string Path { get; set; }
    public ESyncStatus SyncStatus { get; set; }
    public required byte[] Checksum { get; set; }
    public required List<byte[]> RequiredBlocks { get; set; }
}