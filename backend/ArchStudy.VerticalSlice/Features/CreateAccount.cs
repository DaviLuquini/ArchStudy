using ArchStudy.Shared.Dtos;
using ArchStudy.VerticalSlice.Persistence;
using Mediator;

namespace ArchStudy.VerticalSlice.Features;

public sealed record CreateAccountCommand(string Owner) : IRequest<AccountResponse>;

public sealed class CreateAccountHandler(VsDbContext db) : IRequestHandler<CreateAccountCommand, AccountResponse>
{
    public async ValueTask<AccountResponse> Handle(CreateAccountCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Owner)) throw new ArgumentException("Owner required");

        var record = new AccountRecord
        {
            Id = Guid.NewGuid(),
            Owner = command.Owner,
            Balance = 0m,
            CreatedAt = DateTime.UtcNow,
        };
        db.Accounts.Add(record);
        await db.SaveChangesAsync(ct);
        return new AccountResponse(record.Id, record.Owner, record.Balance, record.CreatedAt);
    }
}

public static class CreateAccountEndpoint
{
    public static void Map(RouteGroupBuilder group) =>
        group.MapPost("/accounts", async Task<IResult> (
            CreateAccountRequest req,
            IRequestHandler<CreateAccountCommand, AccountResponse> handler,
            CancellationToken ct) =>
        {
            var account = await handler.Handle(new CreateAccountCommand(req.Owner), ct);
            return Results.Created($"/vertical-slice/accounts/{account.Id}", account);
        });
}
