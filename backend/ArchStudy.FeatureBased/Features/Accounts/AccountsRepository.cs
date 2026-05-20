using ArchStudy.FeatureBased.Shared;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.FeatureBased.Features.Accounts;

public interface IAccountsRepository
{
    Task<AccountRow?> FindAsync(Guid id, CancellationToken ct);
    Task AddAsync(AccountRow account, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}

public class AccountsRepository(FeatureBasedDbContext db) : IAccountsRepository
{
    public Task<AccountRow?> FindAsync(Guid id, CancellationToken ct) =>
        db.Accounts.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task AddAsync(AccountRow account, CancellationToken ct) =>
        await db.Accounts.AddAsync(account, ct);

    public Task SaveAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
