using ArchStudy.Onion.Application;
using ArchStudy.Onion.Infrastructure;
using ArchStudy.Onion.Presentation;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Onion;

public static class OnionModule
{
    public static IServiceCollection AddOnionModule(this IServiceCollection services)
    {
        services.AddDbContext<OnionDbContext>(o => o.UseSqlite("Data Source=archstudy-onion.db"));
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<AccountAppService>();
        services.AddScoped<TransferAppService>();
        return services;
    }

    public static IEndpointRouteBuilder MapOnionModule(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/onion").WithTags("Onion");
        OnionEndpoints.Map(group);
        return routes;
    }

    public static async Task EnsureDatabaseCreatedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<OnionDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
