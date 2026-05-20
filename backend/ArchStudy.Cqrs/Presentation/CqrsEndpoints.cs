using ArchStudy.Cqrs.Commands;
using ArchStudy.Cqrs.Queries;
using ArchStudy.Shared.Dtos;
using Mediator;

namespace ArchStudy.Cqrs.Presentation;

public static class CqrsEndpoints
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/accounts", async Task<IResult> (
            CreateAccountRequest req,
            ICommandHandler<CreateAccountCommand, AccountResponse> handler,
            CancellationToken ct) =>
        {
            var account = await handler.Handle(new CreateAccountCommand(req.Owner), ct);
            return Results.Created($"/cqrs/accounts/{account.Id}", account);
        });

        group.MapGet("/accounts/{id:guid}", async Task<IResult> (
            Guid id,
            IQueryHandler<GetAccountQuery, AccountResponse?> handler,
            CancellationToken ct) =>
        {
            var account = await handler.Handle(new GetAccountQuery(id), ct);
            return account is null ? Results.NotFound() : Results.Ok(account);
        });

        group.MapGet("/accounts/{id:guid}/transactions", async Task<IResult> (
            Guid id,
            IQueryHandler<GetStatementQuery, IReadOnlyList<TransactionResponse>> handler,
            CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetStatementQuery(id), ct)));

        group.MapPost("/accounts/{id:guid}/deposits", async Task<IResult> (
            Guid id,
            DepositRequest req,
            ICommandHandler<DepositCommand, AccountResponse> handler,
            CancellationToken ct) =>
            Results.Ok(await handler.Handle(new DepositCommand(id, req.Amount), ct)));

        group.MapPost("/accounts/{id:guid}/withdrawals", async Task<IResult> (
            Guid id,
            WithdrawalRequest req,
            ICommandHandler<WithdrawCommand, AccountResponse> handler,
            CancellationToken ct) =>
            Results.Ok(await handler.Handle(new WithdrawCommand(id, req.Amount), ct)));

        group.MapPost("/transfers", async Task<IResult> (
            TransferRequest req,
            ICommandHandler<TransferCommand> handler,
            CancellationToken ct) =>
        {
            await handler.Handle(new TransferCommand(req.FromAccountId, req.ToAccountId, req.Amount), ct);
            return Results.NoContent();
        });
    }
}
