using ArchStudy.Shared.Dtos;

namespace ArchStudy.FeatureBased.Features.Accounts;

public static class AccountsController
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/accounts", async Task<IResult> (CreateAccountRequest req, IAccountsService svc, CancellationToken ct) =>
        {
            var account = await svc.CreateAsync(req.Owner, ct);
            return Results.Created($"/feature-based/accounts/{account.Id}", account);
        });

        group.MapGet("/accounts/{id:guid}", async Task<IResult> (Guid id, IAccountsService svc, CancellationToken ct) =>
        {
            var account = await svc.GetAsync(id, ct);
            return account is null ? Results.NotFound() : Results.Ok(account);
        });
    }
}
