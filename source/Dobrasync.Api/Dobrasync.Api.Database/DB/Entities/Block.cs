using Dobrasync.Api.Database.Entities.Base;

namespace Dobrasync.Api.Database.Entities;

public class Block : BaseEntity
{
    public HashSet<File> Files { get; set; } = new();
    public byte[] Checksum { get; set; } = [];
}