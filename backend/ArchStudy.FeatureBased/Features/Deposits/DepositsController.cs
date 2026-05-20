using ArchStudy.Shared.Dtos;

namespace ArchStudy.FeatureBased.Features.Deposits;

public static class DepositsController
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/accounts/{id:guid}/deposits", async Task<IResult> (Guid id, DepositRequest req, IDepositsService svc, CancellationToken ct) =>
            Results.Ok(await svc.DepositAsync(id, req.Amount, ct)));
    }
}
