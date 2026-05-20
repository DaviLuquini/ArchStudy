using ArchStudy.Shared.Dtos;
using ArchStudy.VerticalSlice.Features;
using ArchStudy.VerticalSlice.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.VerticalSlice;

public static class VerticalSliceModule
{
    public static IServiceCollection AddVerticalSliceModule(this IServiceCollection services)
    {
        services.AddDbContext<VsDbContext>(o => o.UseSqlite("Data Source=archstudy-vertical-slice.db"));

        services.AddScoped<IRequestHandler<CreateAccountCommand, AccountResponse>, CreateAccountHandler>();
        services.AddScoped<IRequestHandler<GetAccountQuery, AccountResponse?>, GetAccountHandler>();
        services.AddScoped<IRequestHandler<GetStatementQuery, IReadOnlyList<TransactionResponse>>, GetStatementHandler>();
        services.AddScoped<IRequestHandler<DepositCommand, AccountResponse>, DepositHandler>();
        services.AddScoped<IRequestHandler<WithdrawCommand, AccountResponse>, WithdrawHandler>();
        services.AddScoped<IRequestHandler<TransferCommand>, TransferHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapVerticalSliceModule(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/vertical-slice").WithTags("Vertical Slice");
        CreateAccountEndpoint.Map(group);
        GetAccountEndpoint.Map(group);
        GetStatementEndpoint.Map(group);
        DepositEndpoint.Map(group);
        WithdrawEndpoint.Map(group);
        TransferEndpoint.Map(group);
        return routes;
    }

    public static async Task EnsureDatabaseCreatedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<VsDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
