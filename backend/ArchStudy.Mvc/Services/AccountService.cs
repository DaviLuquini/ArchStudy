using ArchStudy.Mvc.Persistence;
using ArchStudy.Mvc.Repositories;
using ArchStudy.Shared.Dtos;

namespace ArchStudy.Mvc.Services;

public interface IAccountService
{
    Task<AccountResponse> CreateAsync(string owner, CancellationToken ct);
    Task<AccountResponse?> GetAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<TransactionResponse>> GetStatementAsync(Guid id, CancellationToken ct);
    Task<AccountResponse> DepositAsync(Guid id, decimal amount, CancellationToken ct);
    Task<AccountResponse> WithdrawAsync(Guid id, decimal amount, CancellationToken ct);
}

public class AccountService(
    IAccountRepository accounts,
    ITransactionRepository transactions) : IAccountService
{
    public async Task<AccountResponse> CreateAsync(string owner, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(owner))
            throw new ArgumentException("Owner is required", nameof(owner));

        var entity = new AccountEntity
        {
            Id = Guid.NewGuid(),
            Owner = owner,
            Balance = 0m,
            CreatedAt = DateTime.UtcNow,
        };
        await accounts.AddAsync(entity, ct);
        await accounts.SaveChangesAsync(ct);
        return ToResponse(entity);
    }

    public async Task<AccountResponse?> GetAsync(Guid id, CancellationToken ct)
    {
        var entity = await accounts.GetByIdAsync(id, ct);
        return entity is null ? null : ToResponse(entity);
    }

    public async Task<IReadOnlyList<TransactionResponse>> GetStatementAsync(Guid id, CancellationToken ct)
    {
        var list = await transactions.ListByAccountAsync(id, ct);
        return list.Select(t => new TransactionResponse(
            t.Id, t.AccountId, t.Type, t.Amount, t.Timestamp, t.RelatedAccountId)).ToList();
    }

    public async Task<AccountResponse> DepositAsync(Guid id, decimal amount, CancellationToken ct)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive", nameof(amount));
        var entity = await accounts.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Account not found");

        entity.Balance += amount;
        await transactions.AddAsync(new TransactionEntity
        {
            Id = Guid.NewGuid(),
            AccountId = entity.Id,
            Type = TransactionType.Deposit,
            Amount = amount,
            Timestamp = DateTime.UtcNow,
        }, ct);
        await accounts.SaveChangesAsync(ct);
        return ToResponse(entity);
    }

    public async Task<AccountResponse> WithdrawAsync(Guid id, decimal amount, CancellationToken ct)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive", nameof(amount));
        var entity = await accounts.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Account not found");
        if (entity.Balance < amount) throw new InvalidOperationException("Insufficient funds");

        entity.Balance -= amount;
        await transactions.AddAsync(new TransactionEntity
        {
            Id = Guid.NewGuid(),
            AccountId = entity.Id,
            Type = TransactionType.Withdrawal,
            Amount = amount,
            Timestamp = DateTime.UtcNow,
        }, ct);
        await accounts.SaveChangesAsync(ct);
        return ToResponse(entity);
    }

    private static AccountResponse ToResponse(AccountEntity a) =>
        new(a.Id, a.Owner, a.Balance, a.CreatedAt);
}
