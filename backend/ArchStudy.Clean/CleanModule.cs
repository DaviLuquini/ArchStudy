using ArchStudy.Clean.Adapters.Persistence;
using ArchStudy.Clean.Frameworks.Web;
using ArchStudy.Clean.UseCases;
using ArchStudy.Clean.UseCases.Common;
using ArchStudy.Shared.Dtos;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Clean;

public static class CleanModule
{
    public static IServiceCollection AddCleanModule(this IServiceCollection services)
    {
        services.AddDbContext<CleanDbContext>(o => o.UseSqlite("Data Source=archstudy-clean.db"));
        services.AddScoped<IAccountGateway, EfAccountGateway>();
        services.AddScoped<ITransactionGateway, EfTransactionGateway>();
        services.AddScoped<IPersistenceContext, EfPersistenceContext>();

        services.AddScoped<IRequestHandler<CreateAccountInput, UseCaseResult<AccountResponse>>, CreateAccountUseCase>();
        services.AddScoped<IRequestHandler<GetAccountInput, UseCaseResult<AccountResponse>>, GetAccountUseCase>();
        services.AddScoped<IRequestHandler<GetStatementInput, UseCaseResult<IReadOnlyList<TransactionResponse>>>, GetStatementUseCase>();
        services.AddScoped<IRequestHandler<DepositInput, UseCaseResult<AccountResponse>>, DepositUseCase>();
        services.AddScoped<IRequestHandler<WithdrawInput, UseCaseResult<AccountResponse>>, WithdrawUseCase>();
        services.AddScoped<IRequestHandler<TransferInput, UseCaseResult<Unit>>, TransferUseCase>();
        return services;
    }

    public static IEndpointRouteBuilder MapCleanModule(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/clean").WithTags("Clean");
        CleanEndpoints.Map(group);
        return routes;
    }

    public static async Task EnsureDatabaseCreatedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<CleanDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
