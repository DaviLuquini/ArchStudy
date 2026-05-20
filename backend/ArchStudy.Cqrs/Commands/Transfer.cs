using ArchStudy.Cqrs.Domain;
using ArchStudy.Cqrs.Infrastructure;
using ArchStudy.Shared.Dtos;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Cqrs.Commands;

public sealed record TransferCommand(Guid FromAccountId, Guid ToAccountId, decimal Amount) : ICommand;

public sealed class TransferHandler(CqrsWriteDbContext db) : ICommandHandler<TransferCommand>
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
        db.Transactions.Add(new TransactionWriteModel { Id = Guid.NewGuid(), AccountId = from.Id, Type = TransactionType.TransferOut, Amount = command.Amount, Timestamp = now, RelatedAccountId = to.Id });
        db.Transactions.Add(new TransactionWriteModel { Id = Guid.NewGuid(), AccountId = to.Id, Type = TransactionType.TransferIn, Amount = command.Amount, Timestamp = now, RelatedAccountId = from.Id });

        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
