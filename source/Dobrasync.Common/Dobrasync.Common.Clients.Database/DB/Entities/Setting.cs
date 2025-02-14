using Dobrasync.Common.Clients.Database.DB.Entities.Base;

namespace Dobrasync.Common.Clients.Database.DB.Entities;

public class Setting : BaseEntity
{
    public required ESettingKey Key { get; set; }
    public required string Value { get; set; } = string.Empty;
}