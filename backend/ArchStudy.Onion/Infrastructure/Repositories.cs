using ArchStudy.Onion.Application;
using ArchStudy.Onion.Domain;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Onion.Infrastructure;

public class AccountRepository(OnionDbContext db) : IAccountRepository
{
    public Task<Account?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Accounts.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task AddAsync(Account account, CancellationToken ct) =>
        await db.Accounts.AddAsync(account, ct);
}

public class TransactionRepository(OnionDbContext db) : ITransactionRepository
{
    public async Task<IReadOnlyList<Transaction>> ListByAccountAsync(Guid accountId, CancellationToken ct) =>
        await db.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(ct);

    public async Task AddAsync(Transaction transaction, CancellationToken ct) =>
        await db.Transactions.AddAsync(transaction, ct);
}

public class UnitOfWork(OnionDbContext db) : IUnitOfWork
{
    public Task SaveAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
