using Dobrasync.Api.Database.DB;
using Dobrasync.Api.Database.Entities;
using File = Dobrasync.Api.Database.Entities.File;
using Version = Dobrasync.Api.Database.Entities.Version;

namespace Dobrasync.Api.Database.Repos;

public interface IRepoWrapper
{
    DobrasyncContext DbContext { get; }
    IRepo<Block> BlockRepo { get; }
    IRepo<Library> LibraryRepo { get; }
    IRepo<File> FileRepo { get; }
    IRepo<Version> VersionRepo { get; }
    IRepo<Client> ClientRepo { get; }
    IRepo<OrderedBlock> OrderedBlockRepo { get; }
}