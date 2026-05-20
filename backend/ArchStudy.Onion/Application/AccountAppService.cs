using ArchStudy.Onion.Domain;
using ArchStudy.Shared.Dtos;

namespace ArchStudy.Onion.Application;

public class AccountAppService(
    IAccountRepository accounts,
    ITransactionRepository transactions,
    IUnitOfWork uow)
{
    public async Task<AccountResponse> CreateAsync(string owner, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(owner)) throw new ArgumentException("Owner required");
        var account = Account.Create(owner);
        await accounts.AddAsync(account, ct);
        await uow.SaveAsync(ct);
        return Map(account);
    }

    public async Task<AccountResponse?> GetAsync(Guid id, CancellationToken ct)
    {
        var account = await accounts.GetByIdAsync(id, ct);
        return account is null ? null : Map(account);
    }

    public async Task<IReadOnlyList<TransactionResponse>> GetStatementAsync(Guid id, CancellationToken ct)
    {
        var rows = await transactions.ListByAccountAsync(id, ct);
        return rows.Select(t => new TransactionResponse(t.Id, t.AccountId, t.Type, t.Amount, t.Timestamp, t.RelatedAccountId)).ToList();
    }

    public async Task<AccountResponse> DepositAsync(Guid id, decimal amount, CancellationToken ct)
    {
        var account = await accounts.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Account not found");
        account.Deposit(amount);
        await transactions.AddAsync(Transaction.Create(account.Id, TransactionType.Deposit, amount), ct);
        await uow.SaveAsync(ct);
        return Map(account);
    }

    public async Task<AccountResponse> WithdrawAsync(Guid id, decimal amount, CancellationToken ct)
    {
        var account = await accounts.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Account not found");
        account.Withdraw(amount);
        await transactions.AddAsync(Transaction.Create(account.Id, TransactionType.Withdrawal, amount), ct);
        await uow.SaveAsync(ct);
        return Map(account);
    }

    private static AccountResponse Map(Account a) => new(a.Id, a.Owner, a.Balance, a.CreatedAt);
}

public class TransferAppService(IAccountRepository accounts, ITransactionRepository transactions, IUnitOfWork uow)
{
    public async Task TransferAsync(Guid fromId, Guid toId, decimal amount, CancellationToken ct)
    {
        if (fromId == toId) throw new ArgumentException("Accounts must differ");
        var from = await accounts.GetByIdAsync(fromId, ct) ?? throw new KeyNotFoundException("Source account not found");
        var to = await accounts.GetByIdAsync(toId, ct) ?? throw new KeyNotFoundException("Destination account not found");

        from.Withdraw(amount);
        to.Deposit(amount);

        await transactions.AddAsync(Transaction.Create(from.Id, TransactionType.TransferOut, amount, to.Id), ct);
        await transactions.AddAsync(Transaction.Create(to.Id, TransactionType.TransferIn, amount, from.Id), ct);
        await uow.SaveAsync(ct);
    }
}
