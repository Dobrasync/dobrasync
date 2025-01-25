using Dobrasync.Api.Database.DB;
using Dobrasync.Api.Database.Entities;
using Microsoft.AspNetCore.Http;
using File = Dobrasync.Api.Database.Entities.File;

namespace Dobrasync.Api.Database.Repos;

public class RepoWrapper(DobrasyncContext context, IHttpContextAccessor hca) : IRepoWrapper
{
    private IRepo<Block> _blockRepo = null!;
    private IRepo<File> _fileRepo = null!;
    private IRepo<Library> _libraryRepo = null!;
    private IRepo<Client> _clientRepo = null!;
    private IRepo<Transaction> _transactionRepo = null!;

    public DobrasyncContext DbContext => context;

    #region Repos
    public IRepo<File> FileRepo
    {
        get { return _fileRepo ??= new Repo<File>(context, hca); }
    }

    public IRepo<Library> LibraryRepo
    {
        get { return _libraryRepo ??= new Repo<Library>(context, hca); }
    }

    public IRepo<Block> BlockRepo
    {
        get { return _blockRepo ??= new Repo<Block>(context, hca); }
    }
    
    public IRepo<Transaction> TransactionRepo
    {
        get { return _transactionRepo ??= new Repo<Transaction>(context, hca); }
    }
    
    public IRepo<Client> ClientRepo
    {
        get { return _clientRepo ??= new Repo<Client>(context, hca); }
    }
    #endregion
}