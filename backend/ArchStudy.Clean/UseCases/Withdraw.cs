using ArchStudy.Clean.Entities;
using ArchStudy.Clean.UseCases.Common;
using ArchStudy.Shared.Dtos;
using Mediator;

namespace ArchStudy.Clean.UseCases;

public sealed record WithdrawInput(Guid AccountId, decimal Amount) : IRequest<UseCaseResult<AccountResponse>>;

public sealed class WithdrawUseCase(
    IAccountGateway accounts,
    ITransactionGateway transactions,
    IPersistenceContext context)
    : IRequestHandler<WithdrawInput, UseCaseResult<AccountResponse>>
{
    public async ValueTask<UseCaseResult<AccountResponse>> Handle(WithdrawInput input, CancellationToken ct)
    {
        if (input.Amount <= 0) return new UseCaseResult<AccountResponse>.Invalid("Amount must be positive");

        var account = await accounts.FindAsync(input.AccountId, ct);
        if (account is null) return new UseCaseResult<AccountResponse>.NotFound("Account not found");

        if (!account.TryWithdraw(input.Amount))
            return new UseCaseResult<AccountResponse>.Invalid("Insufficient funds");

        await transactions.RecordAsync(Transaction.Create(account.Id, TransactionType.Withdrawal, input.Amount, DateTime.UtcNow), ct);
        await context.CommitAsync(ct);

        return new UseCaseResult<AccountResponse>.Ok(new AccountResponse(account.Id, account.Owner, account.Balance, account.CreatedAt));
    }
}
