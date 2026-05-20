using ArchStudy.Clean.Entities;
using ArchStudy.Clean.UseCases.Common;
using ArchStudy.Shared.Dtos;
using Mediator;

namespace ArchStudy.Clean.UseCases;

public sealed record TransferInput(Guid FromAccountId, Guid ToAccountId, decimal Amount) : IRequest<UseCaseResult<Unit>>;

public sealed class TransferUseCase(
    IAccountGateway accounts,
    ITransactionGateway transactions,
    IPersistenceContext context)
    : IRequestHandler<TransferInput, UseCaseResult<Unit>>
{
    public async ValueTask<UseCaseResult<Unit>> Handle(TransferInput input, CancellationToken ct)
    {
        if (input.Amount <= 0) return new UseCaseResult<Unit>.Invalid("Amount must be positive");
        if (input.FromAccountId == input.ToAccountId) return new UseCaseResult<Unit>.Invalid("Accounts must differ");

        var from = await accounts.FindAsync(input.FromAccountId, ct);
        if (from is null) return new UseCaseResult<Unit>.NotFound("Source account not found");
        var to = await accounts.FindAsync(input.ToAccountId, ct);
        if (to is null) return new UseCaseResult<Unit>.NotFound("Destination account not found");

        if (!from.TryWithdraw(input.Amount)) return new UseCaseResult<Unit>.Invalid("Insufficient funds");
        to.TryDeposit(input.Amount);

        var now = DateTime.UtcNow;
        await transactions.RecordAsync(Transaction.Create(from.Id, TransactionType.TransferOut, input.Amount, now, to.Id), ct);
        await transactions.RecordAsync(Transaction.Create(to.Id, TransactionType.TransferIn, input.Amount, now, from.Id), ct);
        await context.CommitAsync(ct);

        return new UseCaseResult<Unit>.Ok(Unit.Value);
    }
}
