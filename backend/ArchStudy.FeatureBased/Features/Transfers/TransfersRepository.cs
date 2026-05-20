using ArchStudy.FeatureBased.Shared;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.FeatureBased.Features.Transfers;

public interface ITransfersRepository
{
    Task<AccountRow?> FindAccountAsync(Guid id, CancellationToken ct);
    Task AddTransactionAsync(TransactionRow tx, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}

public class TransfersRepository(FeatureBasedDbContext db) : ITransfersRepository
{
    public Task<AccountRow?> FindAccountAsync(Guid id, CancellationToken ct) =>
        db.Accounts.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task AddTransactionAsync(TransactionRow tx, CancellationToken ct) =>
        await db.Transactions.AddAsync(tx, ct);

    public Task SaveAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
