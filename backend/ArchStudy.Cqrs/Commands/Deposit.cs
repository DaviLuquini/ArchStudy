using ArchStudy.Cqrs.Domain;
using ArchStudy.Cqrs.Infrastructure;
using ArchStudy.Shared.Dtos;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Cqrs.Commands;

public sealed record DepositCommand(Guid AccountId, decimal Amount) : ICommand<AccountResponse>;

public sealed class DepositHandler(CqrsWriteDbContext db) : ICommandHandler<DepositCommand, AccountResponse>
{
    public async ValueTask<AccountResponse> Handle(DepositCommand command, CancellationToken ct)
    {
        if (command.Amount <= 0) throw new ArgumentException("Amount must be positive");

        var account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == command.AccountId, ct)
            ?? throw new KeyNotFoundException("Account not found");

        account.Balance += command.Amount;
        db.Transactions.Add(new TransactionWriteModel
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
