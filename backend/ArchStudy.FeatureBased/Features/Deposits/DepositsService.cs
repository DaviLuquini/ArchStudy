using ArchStudy.FeatureBased.Shared;
using ArchStudy.Shared.Dtos;

namespace ArchStudy.FeatureBased.Features.Deposits;

public interface IDepositsService
{
    Task<AccountResponse> DepositAsync(Guid accountId, decimal amount, CancellationToken ct);
}

public class DepositsService(IDepositsRepository repo) : IDepositsService
{
    public async Task<AccountResponse> DepositAsync(Guid accountId, decimal amount, CancellationToken ct)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive");
        var account = await repo.FindAccountAsync(accountId, ct) ?? throw new KeyNotFoundException("Account not found");

        account.Balance += amount;
        await repo.AddTransactionAsync(new TransactionRow
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Type = TransactionType.Deposit,
            Amount = amount,
            Timestamp = DateTime.UtcNow,
        }, ct);
        await repo.SaveAsync(ct);
        return new AccountResponse(account.Id, account.Owner, account.Balance, account.CreatedAt);
    }
}
