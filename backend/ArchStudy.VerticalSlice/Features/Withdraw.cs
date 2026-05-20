using ArchStudy.Shared.Dtos;
using ArchStudy.VerticalSlice.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.VerticalSlice.Features;

public sealed record WithdrawCommand(Guid AccountId, decimal Amount) : IRequest<AccountResponse>;

public sealed class WithdrawHandler(VsDbContext db) : IRequestHandler<WithdrawCommand, AccountResponse>
{
    public async ValueTask<AccountResponse> Handle(WithdrawCommand command, CancellationToken ct)
    {
        if (command.Amount <= 0) throw new ArgumentException("Amount must be positive");

        var account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == command.AccountId, ct)
            ?? throw new KeyNotFoundException("Account not found");
        if (account.Balance < command.Amount) throw new InvalidOperationException("Insufficient funds");

        account.Balance -= command.Amount;
        db.Transactions.Add(new TransactionRecord
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Type = TransactionType.Withdrawal,
            Amount = command.Amount,
            Timestamp = DateTime.UtcNow,
        });

        await db.SaveChangesAsync(ct);
        return new AccountResponse(account.Id, account.Owner, account.Balance, account.CreatedAt);
    }
}

public static class WithdrawEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapPost("/accounts/{id:guid}/withdrawals", async Task<IResult> (
            Guid id,
            WithdrawalRequest req,
            IRequestHandler<WithdrawCommand, AccountResponse> handler,
            CancellationToken ct) =>
            Results.Ok(await handler.Handle(new WithdrawCommand(id, req.Amount), ct)));
}
