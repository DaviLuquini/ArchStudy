using ArchStudy.Shared.Dtos;
using ArchStudy.VerticalSlice.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.VerticalSlice.Features;

public sealed record GetStatementQuery(Guid AccountId) : IRequest<IReadOnlyList<TransactionResponse>>;

public sealed class GetStatementHandler(VsDbContext db) : IRequestHandler<GetStatementQuery, IReadOnlyList<TransactionResponse>>
{
    public async ValueTask<IReadOnlyList<TransactionResponse>> Handle(GetStatementQuery query, CancellationToken ct)
    {
        var rows = await db.Transactions
            .Where(t => t.AccountId == query.AccountId)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(ct);

        return rows.Select(r => new TransactionResponse(r.Id, r.AccountId, r.Type, r.Amount, r.Timestamp, r.RelatedAccountId)).ToList();
    }
}

public static class GetStatementEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapGet("/accounts/{id:guid}/transactions", async Task<IResult> (
            Guid id,
            IRequestHandler<GetStatementQuery, IReadOnlyList<TransactionResponse>> handler,
            CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetStatementQuery(id), ct)));
}
