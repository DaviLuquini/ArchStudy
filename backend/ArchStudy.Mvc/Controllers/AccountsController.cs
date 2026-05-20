using ArchStudy.Mvc.Services;
using ArchStudy.Shared.Dtos;

namespace ArchStudy.Mvc.Controllers;

public static class AccountsController
{
    public static void MapEndpoints(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/mvc").WithTags("MVC");

        group.MapPost("/accounts", CreateAccount);
        group.MapGet("/accounts/{id:guid}", GetAccount);
        group.MapGet("/accounts/{id:guid}/transactions", GetStatement);
        group.MapPost("/accounts/{id:guid}/deposits", Deposit);
        group.MapPost("/accounts/{id:guid}/withdrawals", Withdraw);
        group.MapPost("/transfers", Transfer);
    }

    private static async Task<IResult> CreateAccount(CreateAccountRequest req, IAccountService svc, CancellationToken ct)
    {
        var account = await svc.CreateAsync(req.Owner, ct);
        return Results.Created($"/mvc/accounts/{account.Id}", account);
    }

    private static async Task<IResult> GetAccount(Guid id, IAccountService svc, CancellationToken ct)
    {
        var account = await svc.GetAsync(id, ct);
        return account is null ? Results.NotFound() : Results.Ok(account);
    }

    private static async Task<IResult> GetStatement(Guid id, IAccountService svc, CancellationToken ct) =>
        Results.Ok(await svc.GetStatementAsync(id, ct));

    private static async Task<IResult> Deposit(Guid id, DepositRequest req, IAccountService svc, CancellationToken ct) =>
        Results.Ok(await svc.DepositAsync(id, req.Amount, ct));

    private static async Task<IResult> Withdraw(Guid id, WithdrawalRequest req, IAccountService svc, CancellationToken ct) =>
        Results.Ok(await svc.WithdrawAsync(id, req.Amount, ct));

    private static async Task<IResult> Transfer(TransferRequest req, ITransferService svc, CancellationToken ct)
    {
        await svc.TransferAsync(req.FromAccountId, req.ToAccountId, req.Amount, ct);
        return Results.NoContent();
    }
}
