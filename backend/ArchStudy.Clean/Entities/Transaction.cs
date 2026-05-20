using ArchStudy.Shared.Dtos;

namespace ArchStudy.Clean.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Timestamp { get; private set; }
    public Guid? RelatedAccountId { get; private set; }

    private Transaction() { }

    public static Transaction Create(Guid accountId, TransactionType type, decimal amount, DateTime now, Guid? related = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Type = type,
            Amount = amount,
            Timestamp = now,
            RelatedAccountId = related,
        };
}
