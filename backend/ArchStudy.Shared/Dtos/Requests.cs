namespace ArchStudy.Shared.Dtos;

public sealed record CreateAccountRequest(string Owner);

public sealed record DepositRequest(decimal Amount);

public sealed record WithdrawalRequest(decimal Amount);

public sealed record TransferRequest(Guid FromAccountId, Guid ToAccountId, decimal Amount);
