using Dobrasync.Api.BusinessLogic.Dtos.Versions;
using Version = Dobrasync.Api.Database.Entities.Version;

namespace Dobrasync.Api.BusinessLogic.Dtos.Diff;

public class DiffFileDescriptionDto
{
    public required string Path { get; set; }
    public required byte[] FileChecksum { get; set; }
    public Guid LatestVersionId { get; set; }
}