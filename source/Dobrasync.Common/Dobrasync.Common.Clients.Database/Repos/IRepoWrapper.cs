using Dobrasync.Common.Clients.Database.DB;
using Dobrasync.Common.Clients.Database.DB.Entities;
using File = Dobrasync.Common.Clients.Database.DB.Entities.File;
using Version = Dobrasync.Common.Clients.Database.DB.Entities.Version;

namespace Dobrasync.Common.Clients.Database.Repos;

public interface IRepoWrapper
{
    DobrasyncContext DbContext { get; }
    IRepo<Setting> SettingRepo { get; }
    IRepo<Library> LibraryRepo { get; }
    IRepo<File> FileRepo { get; }
    IRepo<Version> VersionRepo { get; }
}