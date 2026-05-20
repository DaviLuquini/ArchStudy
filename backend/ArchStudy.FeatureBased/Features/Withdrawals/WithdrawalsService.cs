using ArchStudy.FeatureBased.Shared;
using ArchStudy.Shared.Dtos;

namespace ArchStudy.FeatureBased.Features.Withdrawals;

public interface IWithdrawalsService
{
    Task<AccountResponse> WithdrawAsync(Guid accountId, decimal amount, CancellationToken ct);
}

public class WithdrawalsService(IWithdrawalsRepository repo) : IWithdrawalsService
{
    public async Task<AccountResponse> WithdrawAsync(Guid accountId, decimal amount, CancellationToken ct)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive");
        var account = await repo.FindAccountAsync(accountId, ct) ?? throw new KeyNotFoundException("Account not found");
        if (account.Balance < amount) throw new InvalidOperationException("Insufficient funds");

        account.Balance -= amount;
        await repo.AddTransactionAsync(new TransactionRow
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Type = TransactionType.Withdrawal,
            Amount = amount,
            Timestamp = DateTime.UtcNow,
        }, ct);
        await repo.SaveAsync(ct);
        return new AccountResponse(account.Id, account.Owner, account.Balance, account.CreatedAt);
    }
}
