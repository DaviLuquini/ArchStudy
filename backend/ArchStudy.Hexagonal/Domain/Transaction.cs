using ArchStudy.Shared.Dtos;

namespace ArchStudy.Hexagonal.Domain;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Timestamp { get; private set; }
    public Guid? RelatedAccountId { get; private set; }

    private Transaction() { }

    public static Transaction Record(Guid accountId, TransactionType type, decimal amount, Guid? related = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Type = type,
            Amount = amount,
            Timestamp = DateTime.UtcNow,
            RelatedAccountId = related,
        };
}
