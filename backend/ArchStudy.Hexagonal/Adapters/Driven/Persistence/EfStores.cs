using ArchStudy.Hexagonal.Domain;
using ArchStudy.Hexagonal.Ports.Driven;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Hexagonal.Adapters.Driven.Persistence;

public class EfAccountStore(HexagonalDbContext db) : IAccountStore
{
    public Task<Account?> LoadAsync(Guid id, CancellationToken ct) =>
        db.Accounts.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task SaveAsync(Account account, CancellationToken ct)
    {
        var entry = db.Entry(account);
        if (entry.State == EntityState.Detached)
        {
            await db.Accounts.AddAsync(account, ct);
        }
    }
}

public class EfTransactionStore(HexagonalDbContext db) : ITransactionStore
{
    public async Task<IReadOnlyList<Transaction>> ListByAccountAsync(Guid accountId, CancellationToken ct) =>
        await db.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(ct);

    public async Task RecordAsync(Transaction transaction, CancellationToken ct) =>
        await db.Transactions.AddAsync(transaction, ct);
}

public class EfTransactionalScope(HexagonalDbContext db) : ITransactionalScope
{
    public Task CommitAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
