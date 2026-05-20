using ArchStudy.Ddd.Domain;
using ArchStudy.Shared.Dtos;

namespace ArchStudy.Ddd.Application;

public class AccountApplicationService(
    IAccountRepository accounts,
    ITransactionLog transactionLog,
    IUnitOfWork uow,
    TransferDomainService transferService)
{
    public async Task<AccountResponse> OpenAccountAsync(string owner, CancellationToken ct)
    {
        var account = Account.Open(owner);
        await accounts.AddAsync(account, ct);
        await uow.CommitAsync(ct);
        return Map(account);
    }

    public async Task<AccountResponse?> GetAccountAsync(Guid id, CancellationToken ct) =>
        (await accounts.LoadAsync(id, ct)) is { } a ? Map(a) : null;

    public async Task<IReadOnlyList<TransactionResponse>> GetStatementAsync(Guid id, CancellationToken ct)
    {
        var entries = await transactionLog.ListByAccountAsync(id, ct);
        return entries
            .Select(t => new TransactionResponse(t.Id, t.AccountId, t.Type, t.Amount.Amount, t.Timestamp, t.RelatedAccountId))
            .ToList();
    }

    public async Task<AccountResponse> DepositAsync(Guid id, decimal amount, CancellationToken ct)
    {
        var account = await accounts.LoadAsync(id, ct) ?? throw new KeyNotFoundException("Account not found");
        var money = Money.Of(amount);
        account.Deposit(money);
        await transactionLog.AppendAsync(Transaction.Of(account.Id, TransactionType.Deposit, money), ct);
        await uow.CommitAsync(ct);
        return Map(account);
    }

    public async Task<AccountResponse> WithdrawAsync(Guid id, decimal amount, CancellationToken ct)
    {
        var account = await accounts.LoadAsync(id, ct) ?? throw new KeyNotFoundException("Account not found");
        var money = Money.Of(amount);
        account.Withdraw(money);
        await transactionLog.AppendAsync(Transaction.Of(account.Id, TransactionType.Withdrawal, money), ct);
        await uow.CommitAsync(ct);
        return Map(account);
    }

    public async Task TransferAsync(Guid fromId, Guid toId, decimal amount, CancellationToken ct)
    {
        var from = await accounts.LoadAsync(fromId, ct) ?? throw new KeyNotFoundException("Source account not found");
        var to = await accounts.LoadAsync(toId, ct) ?? throw new KeyNotFoundException("Destination account not found");
        var money = Money.Of(amount);
        transferService.Transfer(from, to, money);

        await transactionLog.AppendAsync(Transaction.Of(from.Id, TransactionType.TransferOut, money, to.Id), ct);
        await transactionLog.AppendAsync(Transaction.Of(to.Id, TransactionType.TransferIn, money, from.Id), ct);
        await uow.CommitAsync(ct);
    }

    private static AccountResponse Map(Account a) => new(a.Id, a.Owner, a.Balance.Amount, a.CreatedAt);
}
