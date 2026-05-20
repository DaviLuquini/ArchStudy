using ArchStudy.Cqrs.Infrastructure;
using ArchStudy.Shared.Dtos;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Cqrs.Queries;

public sealed record GetAccountQuery(Guid Id) : IQuery<AccountResponse?>;

public sealed class GetAccountHandler(CqrsReadDbContext db) : IQueryHandler<GetAccountQuery, AccountResponse?>
{
    public async ValueTask<AccountResponse?> Handle(GetAccountQuery query, CancellationToken ct)
    {
        var account = await db.Accounts
            .AsNoTracking()
            .Where(a => a.Id == query.Id)
            .Select(a => new AccountResponse(a.Id, a.Owner, a.Balance, a.CreatedAt))
            .FirstOrDefaultAsync(ct);
        return account;
    }
}
