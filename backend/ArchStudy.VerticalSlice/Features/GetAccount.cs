using ArchStudy.Shared.Dtos;
using ArchStudy.VerticalSlice.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.VerticalSlice.Features;

public sealed record GetAccountQuery(Guid Id) : IRequest<AccountResponse?>;

public sealed class GetAccountHandler(VsDbContext db) : IRequestHandler<GetAccountQuery, AccountResponse?>
{
    public async ValueTask<AccountResponse?> Handle(GetAccountQuery query, CancellationToken ct)
    {
        var record = await db.Accounts.FirstOrDefaultAsync(a => a.Id == query.Id, ct);
        return record is null ? null : new AccountResponse(record.Id, record.Owner, record.Balance, record.CreatedAt);
    }
}

public static class GetAccountEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapGet("/accounts/{id:guid}", async Task<IResult> (
            Guid id,
            IRequestHandler<GetAccountQuery, AccountResponse?> handler,
            CancellationToken ct) =>
        {
            var account = await handler.Handle(new GetAccountQuery(id), ct);
            return account is null ? Results.NotFound() : Results.Ok(account);
        });
}
