using ArchStudy.Ddd.Domain;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Ddd.Infrastructure;

public class AccountRepository(DddDbContext db) : IAccountRepository
{
    public Task<Account?> LoadAsync(Guid id, CancellationToken ct) =>
        db.Accounts.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task AddAsync(Account account, CancellationToken ct) =>
        await db.Accounts.AddAsync(account, ct);
}

public class TransactionLog(DddDbContext db) : ITransactionLog
{
    public async Task<IReadOnlyList<Transaction>> ListByAccountAsync(Guid accountId, CancellationToken ct) =>
        await db.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(ct);

    public async Task AppendAsync(Transaction transaction, CancellationToken ct) =>
        await db.Transactions.AddAsync(transaction, ct);
}

public class UnitOfWork(DddDbContext db) : IUnitOfWork
{
    public Task CommitAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
