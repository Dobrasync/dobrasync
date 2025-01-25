using Dobrasync.Api.Database.Entities.Base;
using Dobrasync.Api.Shared.Enums;

namespace Dobrasync.Api.Database.Entities;

public class Transaction : BaseEntity
{
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset ModifiedOnUtc { get; set; }
    public ETransactionStatus Status { get; set; }
    public HashSet<File> Files { get; set; } = new();
}