using ArchStudy.Shared.Dtos;

namespace ArchStudy.Cqrs.Domain;

public class AccountWriteModel
{
    public Guid Id { get; set; }
    public string Owner { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TransactionWriteModel
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid? RelatedAccountId { get; set; }
}
