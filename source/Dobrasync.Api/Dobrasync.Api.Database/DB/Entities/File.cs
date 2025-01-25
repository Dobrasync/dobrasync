using Dobrasync.Api.Database.Entities.Base;

namespace Dobrasync.Api.Database.Entities;

public class File : BaseEntity
{
    public required string Path { get; set; }
    public required Library Library { get; set; }
    public List<Block> Blocks { get; set; } = new();
    public HashSet<Transaction> Transactions { get; set; } = new();
    public byte[] Checksum { get; set; } = [];
    #region metadata
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset ModifiedOnUtc { get; set; }
    public int PermissionsOctal { get; set; }
    public bool IsDirectory { get; set; }
    #endregion
}