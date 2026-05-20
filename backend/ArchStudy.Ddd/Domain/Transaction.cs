using ArchStudy.Shared.Dtos;

namespace ArchStudy.Ddd.Domain;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public TransactionType Type { get; private set; }
    public Money Amount { get; private set; }
    public DateTime Timestamp { get; private set; }
    public Guid? RelatedAccountId { get; private set; }

    private Transaction() { }

    public static Transaction Of(Guid accountId, TransactionType type, Money amount, Guid? relatedAccountId = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Type = type,
            Amount = amount,
            Timestamp = DateTime.UtcNow,
            RelatedAccountId = relatedAccountId,
        };
}
