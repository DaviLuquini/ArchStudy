using ArchStudy.Shared.Dtos;

namespace ArchStudy.Hexagonal.Ports.Driving;

public interface IAccountUseCases
{
    Task<AccountResponse> OpenAccountAsync(string owner, CancellationToken ct);
    Task<AccountResponse?> GetAccountAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<TransactionResponse>> GetStatementAsync(Guid accountId, CancellationToken ct);
    Task<AccountResponse> DepositAsync(Guid accountId, decimal amount, CancellationToken ct);
    Task<AccountResponse> WithdrawAsync(Guid accountId, decimal amount, CancellationToken ct);
}

public interface ITransferUseCase
{
    Task TransferAsync(Guid fromId, Guid toId, decimal amount, CancellationToken ct);
}
