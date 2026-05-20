using ArchStudy.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.FeatureBased.Shared;

public class AccountRow
{
    public Guid Id { get; set; }
    public string Owner { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TransactionRow
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid? RelatedAccountId { get; set; }
}

public class FeatureBasedDbContext(DbContextOptions<FeatureBasedDbContext> options) : DbContext(options)
{
    public DbSet<AccountRow> Accounts => Set<AccountRow>();
    public DbSet<TransactionRow> Transactions => Set<TransactionRow>();
}
