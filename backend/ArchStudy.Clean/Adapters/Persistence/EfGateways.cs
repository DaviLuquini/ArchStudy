using ArchStudy.Clean.Entities;
using ArchStudy.Clean.UseCases.Common;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Clean.Adapters.Persistence;

public class EfAccountGateway(CleanDbContext db) : IAccountGateway
{
    public Task<Account?> FindAsync(Guid id, CancellationToken ct) =>
        db.Accounts.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task SaveAsync(Account account, CancellationToken ct)
    {
        if (db.Entry(account).State == EntityState.Detached)
            await db.Accounts.AddAsync(account, ct);
    }
}

public class EfTransactionGateway(CleanDbContext db) : ITransactionGateway
{
    public async Task<IReadOnlyList<Transaction>> ListAsync(Guid accountId, CancellationToken ct) =>
        await db.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(ct);

    public async Task RecordAsync(Transaction transaction, CancellationToken ct) =>
        await db.Transactions.AddAsync(transaction, ct);
}

public class EfPersistenceContext(CleanDbContext db) : IPersistenceContext
{
    public Task CommitAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
