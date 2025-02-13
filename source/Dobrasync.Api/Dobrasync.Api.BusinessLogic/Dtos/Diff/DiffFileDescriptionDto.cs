using Dobrasync.Api.BusinessLogic.Dtos.Versions;
using Version = Dobrasync.Api.Database.Entities.Version;

namespace Dobrasync.Api.BusinessLogic.Dtos.Diff;

public class DiffFileDescriptionDto
{
    public required string Path { get; set; }
    public required string FileChecksum { get; set; }
    /// <summary>
    /// Null version means the file is new on client
    /// </summary>
    public Guid? LatestVersionId { get; set; }
}