using ArchStudy.Hexagonal.Adapters.Driven.Persistence;
using ArchStudy.Hexagonal.Adapters.Driving.Http;
using ArchStudy.Hexagonal.Application;
using ArchStudy.Hexagonal.Ports.Driven;
using ArchStudy.Hexagonal.Ports.Driving;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Hexagonal;

public static class HexagonalModule
{
    public static IServiceCollection AddHexagonalModule(this IServiceCollection services)
    {
        services.AddDbContext<HexagonalDbContext>(o => o.UseSqlite("Data Source=archstudy-hexagonal.db"));

        services.AddScoped<IAccountStore, EfAccountStore>();
        services.AddScoped<ITransactionStore, EfTransactionStore>();
        services.AddScoped<ITransactionalScope, EfTransactionalScope>();

        services.AddScoped<IAccountUseCases, AccountUseCases>();
        services.AddScoped<ITransferUseCase, TransferUseCase>();
        return services;
    }

    public static IEndpointRouteBuilder MapHexagonalModule(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/hexagonal").WithTags("Hexagonal");
        HexagonalEndpoints.Map(group);
        return routes;
    }

    public static async Task EnsureDatabaseCreatedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<HexagonalDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
