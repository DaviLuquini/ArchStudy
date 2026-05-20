using ArchStudy.Mvc.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Mvc.Repositories;

public interface ITransactionRepository
{
    Task<IReadOnlyList<TransactionEntity>> ListByAccountAsync(Guid accountId, CancellationToken ct);
    Task AddAsync(TransactionEntity transaction, CancellationToken ct);
}

public class TransactionRepository(MvcDbContext db) : ITransactionRepository
{
    public async Task<IReadOnlyList<TransactionEntity>> ListByAccountAsync(Guid accountId, CancellationToken ct) =>
        await db.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(ct);

    public async Task AddAsync(TransactionEntity transaction, CancellationToken ct) =>
        await db.Transactions.AddAsync(transaction, ct);
}
