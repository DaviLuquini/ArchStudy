using ArchStudy.Shared.Dtos;

namespace ArchStudy.FeatureBased.Features.Withdrawals;

public static class WithdrawalsController
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/accounts/{id:guid}/withdrawals", async Task<IResult> (Guid id, WithdrawalRequest req, IWithdrawalsService svc, CancellationToken ct) =>
            Results.Ok(await svc.WithdrawAsync(id, req.Amount, ct)));
    }
}
