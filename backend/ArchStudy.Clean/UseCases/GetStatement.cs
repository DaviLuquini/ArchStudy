using ArchStudy.Clean.UseCases.Common;
using ArchStudy.Shared.Dtos;
using Mediator;

namespace ArchStudy.Clean.UseCases;

public sealed record GetStatementInput(Guid AccountId) : IRequest<UseCaseResult<IReadOnlyList<TransactionResponse>>>;

public sealed class GetStatementUseCase(ITransactionGateway transactions)
    : IRequestHandler<GetStatementInput, UseCaseResult<IReadOnlyList<TransactionResponse>>>
{
    public async ValueTask<UseCaseResult<IReadOnlyList<TransactionResponse>>> Handle(GetStatementInput input, CancellationToken ct)
    {
        var rows = await transactions.ListAsync(input.AccountId, ct);
        IReadOnlyList<TransactionResponse> mapped = rows
            .Select(t => new TransactionResponse(t.Id, t.AccountId, t.Type, t.Amount, t.Timestamp, t.RelatedAccountId))
            .ToList();
        return new UseCaseResult<IReadOnlyList<TransactionResponse>>.Ok(mapped);
    }
}
