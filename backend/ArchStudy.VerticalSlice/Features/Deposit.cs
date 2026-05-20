using ArchStudy.Shared.Dtos;
using ArchStudy.VerticalSlice.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.VerticalSlice.Features;

public sealed record DepositCommand(Guid AccountId, decimal Amount) : IRequest<AccountResponse>;

public sealed class DepositHandler(VsDbContext db) : IRequestHandler<DepositCommand, AccountResponse>
{
    public async ValueTask<AccountResponse> Handle(DepositCommand command, CancellationToken ct)
    {
        if (command.Amount <= 0) throw new ArgumentException("Amount must be positive");

        var account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == command.AccountId, ct)
            ?? throw new KeyNotFoundException("Account not found");

        account.Balance += command.Amount;
        db.Transactions.Add(new TransactionRecord
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Type = TransactionType.Deposit,
            Amount = command.Amount,
            Timestamp = DateTime.UtcNow,
        });

        await db.SaveChangesAsync(ct);
        return new AccountResponse(account.Id, account.Owner, account.Balance, account.CreatedAt);
    }
}

public static class DepositEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapPost("/accounts/{id:guid}/deposits", async Task<IResult> (
            Guid id,
            DepositRequest req,
            IRequestHandler<DepositCommand, AccountResponse> handler,
            CancellationToken ct) =>
            Results.Ok(await handler.Handle(new DepositCommand(id, req.Amount), ct)));
}
