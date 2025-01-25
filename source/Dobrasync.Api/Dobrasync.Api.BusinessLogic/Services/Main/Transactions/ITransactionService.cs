using Dobrasync.Api.Database.Entities;
using File = Dobrasync.Api.Database.Entities.File;

namespace Dobrasync.Api.BusinessLogic.Services.Main.Transactions;

public interface ITransactionService
{
    public Task<Transaction> CreateAsync(Guid libraryId, List<File> files);
    public Task<Transaction> CompleteAsync(Guid transactionId);
    public Task<Transaction> FailAsync(Guid transactionId);
    public Task<bool> IsFileLocked(Guid file);
}