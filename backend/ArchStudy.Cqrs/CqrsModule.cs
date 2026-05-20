using ArchStudy.Cqrs.Commands;
using ArchStudy.Cqrs.Infrastructure;
using ArchStudy.Cqrs.Presentation;
using ArchStudy.Cqrs.Queries;
using ArchStudy.Shared.Dtos;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Cqrs;

public static class CqrsModule
{
    private const string ConnectionString = "Data Source=archstudy-cqrs.db";

    public static IServiceCollection AddCqrsModule(this IServiceCollection services)
    {
        services.AddDbContext<CqrsWriteDbContext>(o => o.UseSqlite(ConnectionString));
        services.AddDbContext<CqrsReadDbContext>(o => o.UseSqlite(ConnectionString));

        services.AddScoped<ICommandHandler<CreateAccountCommand, AccountResponse>, CreateAccountHandler>();
        services.AddScoped<ICommandHandler<DepositCommand, AccountResponse>, DepositHandler>();
        services.AddScoped<ICommandHandler<WithdrawCommand, AccountResponse>, WithdrawHandler>();
        services.AddScoped<ICommandHandler<TransferCommand>, TransferHandler>();

        services.AddScoped<IQueryHandler<GetAccountQuery, AccountResponse?>, GetAccountHandler>();
        services.AddScoped<IQueryHandler<GetStatementQuery, IReadOnlyList<TransactionResponse>>, GetStatementHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapCqrsModule(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/cqrs").WithTags("CQRS");
        CqrsEndpoints.Map(group);
        return routes;
    }

    public static async Task EnsureDatabaseCreatedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<CqrsWriteDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
