using ArchStudy.Hexagonal.Domain;
using ArchStudy.Hexagonal.Ports.Driven;
using ArchStudy.Hexagonal.Ports.Driving;
using ArchStudy.Shared.Dtos;

namespace ArchStudy.Hexagonal.Application;

public class AccountUseCases(
    IAccountStore accountStore,
    ITransactionStore transactionStore,
    ITransactionalScope scope) : IAccountUseCases
{
    public async Task<AccountResponse> OpenAccountAsync(string owner, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(owner)) throw new ArgumentException("Owner required");
        var account = Account.Open(owner);
        await accountStore.SaveAsync(account, ct);
        await scope.CommitAsync(ct);
        return Map(account);
    }

    public async Task<AccountResponse?> GetAccountAsync(Guid id, CancellationToken ct) =>
        (await accountStore.LoadAsync(id, ct)) is { } a ? Map(a) : null;

    public async Task<IReadOnlyList<TransactionResponse>> GetStatementAsync(Guid accountId, CancellationToken ct)
    {
        var rows = await transactionStore.ListByAccountAsync(accountId, ct);
        return rows.Select(t => new TransactionResponse(t.Id, t.AccountId, t.Type, t.Amount, t.Timestamp, t.RelatedAccountId)).ToList();
    }

    public async Task<AccountResponse> DepositAsync(Guid accountId, decimal amount, CancellationToken ct)
    {
        var account = await accountStore.LoadAsync(accountId, ct) ?? throw new KeyNotFoundException("Account not found");
        account.Credit(amount);
        await transactionStore.RecordAsync(Transaction.Record(account.Id, TransactionType.Deposit, amount), ct);
        await scope.CommitAsync(ct);
        return Map(account);
    }

    public async Task<AccountResponse> WithdrawAsync(Guid accountId, decimal amount, CancellationToken ct)
    {
        var account = await accountStore.LoadAsync(accountId, ct) ?? throw new KeyNotFoundException("Account not found");
        account.Debit(amount);
        await transactionStore.RecordAsync(Transaction.Record(account.Id, TransactionType.Withdrawal, amount), ct);
        await scope.CommitAsync(ct);
        return Map(account);
    }

    private static AccountResponse Map(Account a) => new(a.Id, a.Owner, a.Balance, a.CreatedAt);
}

public class TransferUseCase(
    IAccountStore accountStore,
    ITransactionStore transactionStore,
    ITransactionalScope scope) : ITransferUseCase
{
    public async Task TransferAsync(Guid fromId, Guid toId, decimal amount, CancellationToken ct)
    {
        if (fromId == toId) throw new ArgumentException("Accounts must differ");
        var from = await accountStore.LoadAsync(fromId, ct) ?? throw new KeyNotFoundException("Source account not found");
        var to = await accountStore.LoadAsync(toId, ct) ?? throw new KeyNotFoundException("Destination account not found");

        from.Debit(amount);
        to.Credit(amount);

        await transactionStore.RecordAsync(Transaction.Record(from.Id, TransactionType.TransferOut, amount, to.Id), ct);
        await transactionStore.RecordAsync(Transaction.Record(to.Id, TransactionType.TransferIn, amount, from.Id), ct);
        await scope.CommitAsync(ct);
    }
}
