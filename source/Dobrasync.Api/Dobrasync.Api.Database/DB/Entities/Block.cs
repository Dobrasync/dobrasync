using Dobrasync.Api.Database.Entities.Base;

namespace Dobrasync.Api.Database.Entities;

public class Block : BaseEntity
{
    public HashSet<Version> Versions { get; set; } = new();
    public required string Checksum { get; set; } = string.Empty;
}