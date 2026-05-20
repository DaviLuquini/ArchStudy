using ArchStudy.FeatureBased.Shared;
using ArchStudy.Shared.Dtos;

namespace ArchStudy.FeatureBased.Features.Accounts;

public interface IAccountsService
{
    Task<AccountResponse> CreateAsync(string owner, CancellationToken ct);
    Task<AccountResponse?> GetAsync(Guid id, CancellationToken ct);
}

public class AccountsService(IAccountsRepository repo) : IAccountsService
{
    public async Task<AccountResponse> CreateAsync(string owner, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(owner)) throw new ArgumentException("Owner required");
        var row = new AccountRow { Id = Guid.NewGuid(), Owner = owner, Balance = 0m, CreatedAt = DateTime.UtcNow };
        await repo.AddAsync(row, ct);
        await repo.SaveAsync(ct);
        return new AccountResponse(row.Id, row.Owner, row.Balance, row.CreatedAt);
    }

    public async Task<AccountResponse?> GetAsync(Guid id, CancellationToken ct)
    {
        var row = await repo.FindAsync(id, ct);
        return row is null ? null : new AccountResponse(row.Id, row.Owner, row.Balance, row.CreatedAt);
    }
}
