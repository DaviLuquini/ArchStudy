using ArchStudy.Ddd.Application;
using ArchStudy.Ddd.Domain;
using ArchStudy.Ddd.Infrastructure;
using ArchStudy.Ddd.Presentation;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Ddd;

public static class DddModule
{
    public static IServiceCollection AddDddModule(this IServiceCollection services)
    {
        services.AddDbContext<DddDbContext>(o => o.UseSqlite("Data Source=archstudy-ddd.db"));
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionLog, TransactionLog>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<TransferDomainService>();
        services.AddScoped<AccountApplicationService>();
        return services;
    }

    public static IEndpointRouteBuilder MapDddModule(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/ddd").WithTags("DDD");
        DddEndpoints.Map(group);
        return routes;
    }

    public static async Task EnsureDatabaseCreatedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<DddDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
