using Dobrasync.Api.Database.Entities.Base;

namespace Dobrasync.Api.Database.Entities;

public class File : BaseEntity
{
    public required string Path { get; set; }
    public bool IsDirectory { get; set; }
    public required Library Library { get; set; }
    public List<Block> Blocks { get; set; } = new();
}