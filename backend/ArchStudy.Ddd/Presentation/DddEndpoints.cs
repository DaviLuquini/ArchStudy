using ArchStudy.Ddd.Application;
using ArchStudy.Shared.Dtos;

namespace ArchStudy.Ddd.Presentation;

public static class DddEndpoints
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/accounts", async Task<IResult> (CreateAccountRequest req, AccountApplicationService svc, CancellationToken ct) =>
        {
            var account = await svc.OpenAccountAsync(req.Owner, ct);
            return Results.Created($"/ddd/accounts/{account.Id}", account);
        });

        group.MapGet("/accounts/{id:guid}", async Task<IResult> (Guid id, AccountApplicationService svc, CancellationToken ct) =>
        {
            var account = await svc.GetAccountAsync(id, ct);
            return account is null ? Results.NotFound() : Results.Ok(account);
        });

        group.MapGet("/accounts/{id:guid}/transactions", async Task<IResult> (Guid id, AccountApplicationService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetStatementAsync(id, ct)));

        group.MapPost("/accounts/{id:guid}/deposits", async Task<IResult> (Guid id, DepositRequest req, AccountApplicationService svc, CancellationToken ct) =>
            Results.Ok(await svc.DepositAsync(id, req.Amount, ct)));

        group.MapPost("/accounts/{id:guid}/withdrawals", async Task<IResult> (Guid id, WithdrawalRequest req, AccountApplicationService svc, CancellationToken ct) =>
            Results.Ok(await svc.WithdrawAsync(id, req.Amount, ct)));

        group.MapPost("/transfers", async Task<IResult> (TransferRequest req, AccountApplicationService svc, CancellationToken ct) =>
        {
            await svc.TransferAsync(req.FromAccountId, req.ToAccountId, req.Amount, ct);
            return Results.NoContent();
        });
    }
}
