using ArchStudy.FeatureBased.Shared;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.FeatureBased.Features.Statements;

public interface IStatementsRepository
{
    Task<IReadOnlyList<TransactionRow>> ListByAccountAsync(Guid accountId, CancellationToken ct);
}

public class StatementsRepository(FeatureBasedDbContext db) : IStatementsRepository
{
    public async Task<IReadOnlyList<TransactionRow>> ListByAccountAsync(Guid accountId, CancellationToken ct) =>
        await db.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(ct);
}
