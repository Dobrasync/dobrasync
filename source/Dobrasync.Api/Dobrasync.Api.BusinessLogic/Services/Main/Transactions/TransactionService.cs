using Dobrasync.Api.Database.Entities;
using Dobrasync.Api.Database.Repos;
using Dobrasync.Api.Shared.Enums;
using Dobrasync.Api.Shared.Exceptions.Userspace;
using Microsoft.EntityFrameworkCore;
using File = Dobrasync.Api.Database.Entities.File;

namespace Dobrasync.Api.BusinessLogic.Services.Main.Transactions;

public class TransactionService(IRepoWrapper repo) : ITransactionService
{
    public async Task<Transaction> CreateAsync(Guid libraryId, List<File> files)
    {
        Transaction transaction = new()
        {
            Files = files.ToHashSet(),
            CreatedOnUtc = DateTimeOffset.UtcNow,
            ModifiedOnUtc = DateTimeOffset.UtcNow,
            Status = ETransactionStatus.Pending
        };

        await repo.TransactionRepo.InsertAsync(transaction);
        
        return transaction;
    }

    public async Task<Transaction> CompleteAsync(Guid transactionId)
    {
        Transaction? transaction = await GetTransactionAsync(transactionId);
        
        transaction.ModifiedOnUtc = DateTimeOffset.UtcNow;
        transaction.Status = ETransactionStatus.Success;
        await repo.TransactionRepo.UpdateAsync(transaction);
        
        return transaction;
    }

    public async Task<Transaction> FailAsync(Guid transactionId)
    {
        Transaction? transaction = await GetTransactionAsync(transactionId);
        
        transaction.ModifiedOnUtc = DateTimeOffset.UtcNow;
        transaction.Status = ETransactionStatus.Fail;
        await repo.TransactionRepo.UpdateAsync(transaction);
        
        return transaction;
    }

    public async Task<bool> IsFileLocked(Guid file)
    {
        Transaction? match = await repo.TransactionRepo
            .QueryAll()
            .Include(x => x.Files)
            .Where(x => x.Status == ETransactionStatus.Pending)
            .FirstOrDefaultAsync(x => x.Files.Any(y => y.Id == file));

        return match != null;
    }

    private async Task<Transaction> GetTransactionAsync(Guid transactionId)
    {
        Transaction? transaction = await repo.TransactionRepo.QueryAll().FirstOrDefaultAsync(x => x.Id == transactionId);
        if (transaction == null) throw new NotFoundUSException();
        
        return transaction;
    }
}