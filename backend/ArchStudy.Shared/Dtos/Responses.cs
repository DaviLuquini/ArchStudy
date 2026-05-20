namespace ArchStudy.Shared.Dtos;

public sealed record AccountResponse(
    Guid Id,
    string Owner,
    decimal Balance,
    DateTime CreatedAt);

public enum TransactionType
{
    Deposit,
    Withdrawal,
    TransferIn,
    TransferOut,
}

public sealed record TransactionResponse(
    Guid Id,
    Guid AccountId,
    TransactionType Type,
    decimal Amount,
    DateTime Timestamp,
    Guid? RelatedAccountId);
