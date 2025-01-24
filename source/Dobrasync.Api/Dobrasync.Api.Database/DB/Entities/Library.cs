using Dobrasync.Api.Database.Entities.Base;

namespace Dobrasync.Api.Database.Entities;

public class Library : BaseEntity
{
    public required string Name { get; set; } = string.Empty;
    public virtual HashSet<File> Files { get; set; } = new();
}