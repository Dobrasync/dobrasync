using Dobrasync.Common.Clients.Database.DB;
using Dobrasync.Common.Clients.Database.DB.Entities;

namespace Dobrasync.Common.Clients.Database.Repos;

public class RepoWrapper(DobrasyncContext context) : IRepoWrapper
{
    private IRepo<Setting> _settingRepo = null!;

    public DobrasyncContext DbContext => context;

    #region Repos
    public IRepo<Setting> SettingRepo
    {
        get { return _settingRepo ??= new Repo<Setting>(context); }
    }
    #endregion
}