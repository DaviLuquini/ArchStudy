using ArchStudy.Mvc.Controllers;
using ArchStudy.Mvc.Persistence;
using ArchStudy.Mvc.Repositories;
using ArchStudy.Mvc.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ArchStudy.Mvc;

public static class MvcModule
{
    public static IServiceCollection AddMvcModule(this IServiceCollection services)
    {
        services.AddDbContext<MvcDbContext>(o => o.UseSqlite("Data Source=archstudy-mvc.db"));
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITransferService, TransferService>();
        return services;
    }

    public static IEndpointRouteBuilder MapMvcModule(this IEndpointRouteBuilder routes)
    {
        AccountsController.MapEndpoints(routes);
        return routes;
    }

    public static async Task EnsureDatabaseCreatedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<MvcDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
