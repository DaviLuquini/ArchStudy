using ArchStudy.Mvc.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Mvc.Repositories;

public interface IAccountRepository
{
    Task<AccountEntity?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(AccountEntity account, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

public class AccountRepository(MvcDbContext db) : IAccountRepository
{
    public Task<AccountEntity?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Accounts.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task AddAsync(AccountEntity account, CancellationToken ct) =>
        await db.Accounts.AddAsync(account, ct);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
