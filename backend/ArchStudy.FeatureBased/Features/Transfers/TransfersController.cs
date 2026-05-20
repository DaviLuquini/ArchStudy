using ArchStudy.Shared.Dtos;

namespace ArchStudy.FeatureBased.Features.Transfers;

public static class TransfersController
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/transfers", async Task<IResult> (TransferRequest req, ITransfersService svc, CancellationToken ct) =>
        {
            await svc.TransferAsync(req.FromAccountId, req.ToAccountId, req.Amount, ct);
            return Results.NoContent();
        });
    }
}
