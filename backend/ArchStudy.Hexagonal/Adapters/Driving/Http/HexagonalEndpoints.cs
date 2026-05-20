using ArchStudy.Hexagonal.Ports.Driving;
using ArchStudy.Shared.Dtos;

namespace ArchStudy.Hexagonal.Adapters.Driving.Http;

public static class HexagonalEndpoints
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/accounts", async Task<IResult> (CreateAccountRequest req, IAccountUseCases useCases, CancellationToken ct) =>
        {
            var account = await useCases.OpenAccountAsync(req.Owner, ct);
            return Results.Created($"/hexagonal/accounts/{account.Id}", account);
        });

        group.MapGet("/accounts/{id:guid}", async Task<IResult> (Guid id, IAccountUseCases useCases, CancellationToken ct) =>
        {
            var account = await useCases.GetAccountAsync(id, ct);
            return account is null ? Results.NotFound() : Results.Ok(account);
        });

        group.MapGet("/accounts/{id:guid}/transactions", async Task<IResult> (Guid id, IAccountUseCases useCases, CancellationToken ct) =>
            Results.Ok(await useCases.GetStatementAsync(id, ct)));

        group.MapPost("/accounts/{id:guid}/deposits", async Task<IResult> (Guid id, DepositRequest req, IAccountUseCases useCases, CancellationToken ct) =>
            Results.Ok(await useCases.DepositAsync(id, req.Amount, ct)));

        group.MapPost("/accounts/{id:guid}/withdrawals", async Task<IResult> (Guid id, WithdrawalRequest req, IAccountUseCases useCases, CancellationToken ct) =>
            Results.Ok(await useCases.WithdrawAsync(id, req.Amount, ct)));

        group.MapPost("/transfers", async Task<IResult> (TransferRequest req, ITransferUseCase useCase, CancellationToken ct) =>
        {
            await useCase.TransferAsync(req.FromAccountId, req.ToAccountId, req.Amount, ct);
            return Results.NoContent();
        });
    }
}
