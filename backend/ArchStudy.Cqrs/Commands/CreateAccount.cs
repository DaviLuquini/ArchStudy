using ArchStudy.Cqrs.Domain;
using ArchStudy.Cqrs.Infrastructure;
using ArchStudy.Shared.Dtos;
using Mediator;

namespace ArchStudy.Cqrs.Commands;

public sealed record CreateAccountCommand(string Owner) : ICommand<AccountResponse>;

public sealed class CreateAccountHandler(CqrsWriteDbContext db) : ICommandHandler<CreateAccountCommand, AccountResponse>
{
    public async ValueTask<AccountResponse> Handle(CreateAccountCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Owner)) throw new ArgumentException("Owner required");

        var account = new AccountWriteModel
        {
            Id = Guid.NewGuid(),
            Owner = command.Owner,
            Balance = 0m,
            CreatedAt = DateTime.UtcNow,
        };
        db.Accounts.Add(account);
        await db.SaveChangesAsync(ct);
        return new AccountResponse(account.Id, account.Owner, account.Balance, account.CreatedAt);
    }
}
