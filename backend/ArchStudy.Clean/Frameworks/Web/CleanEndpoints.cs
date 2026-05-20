using ArchStudy.Clean.UseCases;
using ArchStudy.Clean.UseCases.Common;
using ArchStudy.Shared.Dtos;
using Mediator;

namespace ArchStudy.Clean.Frameworks.Web;

public static class CleanEndpoints
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/accounts", async Task<IResult> (
            CreateAccountRequest req,
            IRequestHandler<CreateAccountInput, UseCaseResult<AccountResponse>> handler,
            CancellationToken ct) =>
            ToHttp(await handler.Handle(new CreateAccountInput(req.Owner), ct),
                a => Results.Created($"/clean/accounts/{a.Id}", a)));

        group.MapGet("/accounts/{id:guid}", async Task<IResult> (
            Guid id,
            IRequestHandler<GetAccountInput, UseCaseResult<AccountResponse>> handler,
            CancellationToken ct) =>
            ToHttp(await handler.Handle(new GetAccountInput(id), ct), Results.Ok));

        group.MapGet("/accounts/{id:guid}/transactions", async Task<IResult> (
            Guid id,
            IRequestHandler<GetStatementInput, UseCaseResult<IReadOnlyList<TransactionResponse>>> handler,
            CancellationToken ct) =>
            ToHttp(await handler.Handle(new GetStatementInput(id), ct), Results.Ok));

        group.MapPost("/accounts/{id:guid}/deposits", async Task<IResult> (
            Guid id,
            DepositRequest req,
            IRequestHandler<DepositInput, UseCaseResult<AccountResponse>> handler,
            CancellationToken ct) =>
            ToHttp(await handler.Handle(new DepositInput(id, req.Amount), ct), Results.Ok));

        group.MapPost("/accounts/{id:guid}/withdrawals", async Task<IResult> (
            Guid id,
            WithdrawalRequest req,
            IRequestHandler<WithdrawInput, UseCaseResult<AccountResponse>> handler,
            CancellationToken ct) =>
            ToHttp(await handler.Handle(new WithdrawInput(id, req.Amount), ct), Results.Ok));

        group.MapPost("/transfers", async Task<IResult> (
            TransferRequest req,
            IRequestHandler<TransferInput, UseCaseResult<Unit>> handler,
            CancellationToken ct) =>
            ToHttp(await handler.Handle(new TransferInput(req.FromAccountId, req.ToAccountId, req.Amount), ct),
                _ => Results.NoContent()));
    }

    private static IResult ToHttp<T>(UseCaseResult<T> result, Func<T, IResult> onSuccess) => result switch
    {
        UseCaseResult<T>.Ok ok => onSuccess(ok.Value),
        UseCaseResult<T>.NotFound nf => Results.NotFound(new { error = nf.Message }),
        UseCaseResult<T>.Invalid inv => Results.BadRequest(new { error = inv.Message }),
        _ => Results.StatusCode(500),
    };
}
