using Dobrasync.Common.Clients.Database.DB;
using Dobrasync.Common.Clients.Database.DB.Entities;
using File = Dobrasync.Common.Clients.Database.DB.Entities.File;
using Version = Dobrasync.Common.Clients.Database.DB.Entities.Version;

namespace Dobrasync.Common.Clients.Database.Repos;

public class RepoWrapper(DobrasyncContext context) : IRepoWrapper
{
    private IRepo<Setting> _settingRepo = null!;
    private IRepo<Library> _libRepo = null!;
    private IRepo<File> _fileRepo = null!;
    private IRepo<Version> _versionRepo = null!;

    public DobrasyncContext DbContext => context;

    #region Repos
    public IRepo<Setting> SettingRepo
    {
        get { return _settingRepo ??= new Repo<Setting>(context); }
    }
    
    public IRepo<Library> LibraryRepo
    {
        get { return _libRepo ??= new Repo<Library>(context); }
    }
    
    public IRepo<Version> VersionRepo
    {
        get { return _versionRepo ??= new Repo<Version>(context); }
    }
    
    public IRepo<File> FileRepo
    {
        get { return _fileRepo ??= new Repo<File>(context); }
    }
    #endregion
}