using ArchStudy.Cqrs.Infrastructure;
using ArchStudy.Shared.Dtos;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Cqrs.Queries;

public sealed record GetStatementQuery(Guid AccountId) : IQuery<IReadOnlyList<TransactionResponse>>;

public sealed class GetStatementHandler(CqrsReadDbContext db) : IQueryHandler<GetStatementQuery, IReadOnlyList<TransactionResponse>>
{
    public async ValueTask<IReadOnlyList<TransactionResponse>> Handle(GetStatementQuery query, CancellationToken ct)
    {
        var list = await db.Transactions
            .AsNoTracking()
            .Where(t => t.AccountId == query.AccountId)
            .OrderByDescending(t => t.Timestamp)
            .Select(t => new TransactionResponse(t.Id, t.AccountId, t.Type, t.Amount, t.Timestamp, t.RelatedAccountId))
            .ToListAsync(ct);
        return list;
    }
}
