using Dobrasync.Common.Clients.Database.DB;
using Dobrasync.Common.Clients.Database.DB.Entities;

namespace Dobrasync.Common.Clients.Database.Repos;

public interface IRepoWrapper
{
    DobrasyncContext DbContext { get; }
    IRepo<Setting> SettingRepo { get; }
}