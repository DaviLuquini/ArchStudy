using ArchStudy.Shared.Dtos;
using ArchStudy.VerticalSlice.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.VerticalSlice.Features;

public sealed record TransferCommand(Guid FromAccountId, Guid ToAccountId, decimal Amount) : IRequest;

public sealed class TransferHandler(VsDbContext db) : IRequestHandler<TransferCommand>
{
    public async ValueTask<Unit> Handle(TransferCommand command, CancellationToken ct)
    {
        if (command.Amount <= 0) throw new ArgumentException("Amount must be positive");
        if (command.FromAccountId == command.ToAccountId) throw new ArgumentException("Accounts must differ");

        var from = await db.Accounts.FirstOrDefaultAsync(a => a.Id == command.FromAccountId, ct)
            ?? throw new KeyNotFoundException("Source account not found");
        var to = await db.Accounts.FirstOrDefaultAsync(a => a.Id == command.ToAccountId, ct)
            ?? throw new KeyNotFoundException("Destination account not found");
        if (from.Balance < command.Amount) throw new InvalidOperationException("Insufficient funds");

        from.Balance -= command.Amount;
        to.Balance += command.Amount;
        var now = DateTime.UtcNow;
        db.Transactions.Add(new TransactionRecord { Id = Guid.NewGuid(), AccountId = from.Id, Type = TransactionType.TransferOut, Amount = command.Amount, Timestamp = now, RelatedAccountId = to.Id });
        db.Transactions.Add(new TransactionRecord { Id = Guid.NewGuid(), AccountId = to.Id, Type = TransactionType.TransferIn, Amount = command.Amount, Timestamp = now, RelatedAccountId = from.Id });
        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}

public static class TransferEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapPost("/transfers", async Task<IResult> (
            TransferRequest req,
            IRequestHandler<TransferCommand> handler,
            CancellationToken ct) =>
        {
            await handler.Handle(new TransferCommand(req.FromAccountId, req.ToAccountId, req.Amount), ct);
            return Results.NoContent();
        });
}
