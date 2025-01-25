using Dobrasync.Api.Database.DB;
using Dobrasync.Api.Database.Entities;
using File = Dobrasync.Api.Database.Entities.File;

namespace Dobrasync.Api.Database.Repos;

public interface IRepoWrapper
{
    DobrasyncContext DbContext { get; }
    IRepo<Block> BlockRepo { get; }
    IRepo<Library> LibraryRepo { get; }
    IRepo<File> FileRepo { get; }
    IRepo<Transaction> TransactionRepo { get; }
    IRepo<Client> ClientRepo { get; }
}