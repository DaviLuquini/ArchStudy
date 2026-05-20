using ArchStudy.Clean.UseCases.Common;
using ArchStudy.Shared.Dtos;
using Mediator;

namespace ArchStudy.Clean.UseCases;

public sealed record GetAccountInput(Guid Id) : IRequest<UseCaseResult<AccountResponse>>;

public sealed class GetAccountUseCase(IAccountGateway accounts)
    : IRequestHandler<GetAccountInput, UseCaseResult<AccountResponse>>
{
    public async ValueTask<UseCaseResult<AccountResponse>> Handle(GetAccountInput input, CancellationToken ct)
    {
        var account = await accounts.FindAsync(input.Id, ct);
        return account is null
            ? new UseCaseResult<AccountResponse>.NotFound("Account not found")
            : new UseCaseResult<AccountResponse>.Ok(new AccountResponse(account.Id, account.Owner, account.Balance, account.CreatedAt));
    }
}
