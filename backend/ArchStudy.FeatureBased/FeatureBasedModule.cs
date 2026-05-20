using ArchStudy.FeatureBased.Features.Accounts;
using ArchStudy.FeatureBased.Features.Deposits;
using ArchStudy.FeatureBased.Features.Statements;
using ArchStudy.FeatureBased.Features.Transfers;
using ArchStudy.FeatureBased.Features.Withdrawals;
using ArchStudy.FeatureBased.Shared;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.FeatureBased;

public static class FeatureBasedModule
{
    public static IServiceCollection AddFeatureBasedModule(this IServiceCollection services)
    {
        services.AddDbContext<FeatureBasedDbContext>(o => o.UseSqlite("Data Source=archstudy-feature-based.db"));

        services.AddScoped<IAccountsRepository, AccountsRepository>();
        services.AddScoped<IAccountsService, AccountsService>();
        services.AddScoped<IDepositsRepository, DepositsRepository>();
        services.AddScoped<IDepositsService, DepositsService>();
        services.AddScoped<IWithdrawalsRepository, WithdrawalsRepository>();
        services.AddScoped<IWithdrawalsService, WithdrawalsService>();
        services.AddScoped<ITransfersRepository, TransfersRepository>();
        services.AddScoped<ITransfersService, TransfersService>();
        services.AddScoped<IStatementsRepository, StatementsRepository>();
        services.AddScoped<IStatementsService, StatementsService>();
        return services;
    }

    public static IEndpointRouteBuilder MapFeatureBasedModule(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/feature-based").WithTags("Feature-Based");
        AccountsController.Map(group);
        DepositsController.Map(group);
        WithdrawalsController.Map(group);
        TransfersController.Map(group);
        StatementsController.Map(group);
        return routes;
    }

    public static async Task EnsureDatabaseCreatedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<FeatureBasedDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
