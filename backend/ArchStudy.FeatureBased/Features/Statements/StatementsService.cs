using ArchStudy.FeatureBased.Shared;
using ArchStudy.Shared.Dtos;

namespace ArchStudy.FeatureBased.Features.Statements;

public interface IStatementsService
{
    Task<IReadOnlyList<TransactionResponse>> GetStatementAsync(Guid accountId, CancellationToken ct);
}

public class StatementsService(IStatementsRepository repo) : IStatementsService
{
    public async Task<IReadOnlyList<TransactionResponse>> GetStatementAsync(Guid accountId, CancellationToken ct)
    {
        var rows = await repo.ListByAccountAsync(accountId, ct);
        return rows.Select(r => new TransactionResponse(r.Id, r.AccountId, r.Type, r.Amount, r.Timestamp, r.RelatedAccountId)).ToList();
    }
}
