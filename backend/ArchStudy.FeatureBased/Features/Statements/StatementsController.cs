using ArchStudy.Shared.Dtos;

namespace ArchStudy.FeatureBased.Features.Statements;

public static class StatementsController
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/accounts/{id:guid}/transactions", async Task<IResult> (Guid id, IStatementsService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetStatementAsync(id, ct)));
    }
}
