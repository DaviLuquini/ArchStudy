using ArchStudy.Clean.Entities;
using ArchStudy.Clean.UseCases.Common;
using ArchStudy.Shared.Dtos;
using Mediator;

namespace ArchStudy.Clean.UseCases;

public sealed record CreateAccountInput(string Owner) : IRequest<UseCaseResult<AccountResponse>>;

public sealed class CreateAccountUseCase(
    IAccountGateway accounts,
    IPersistenceContext context)
    : IRequestHandler<CreateAccountInput, UseCaseResult<AccountResponse>>
{
    public async ValueTask<UseCaseResult<AccountResponse>> Handle(CreateAccountInput input, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(input.Owner))
            return new UseCaseResult<AccountResponse>.Invalid("Owner is required");

        var account = Account.Open(input.Owner, DateTime.UtcNow);
        await accounts.SaveAsync(account, ct);
        await context.CommitAsync(ct);

        return new UseCaseResult<AccountResponse>.Ok(
            new AccountResponse(account.Id, account.Owner, account.Balance, account.CreatedAt));
    }
}
