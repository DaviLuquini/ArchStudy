using ArchStudy.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.VerticalSlice.Persistence;

public class AccountRecord
{
    public Guid Id { get; set; }
    public string Owner { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TransactionRecord
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid? RelatedAccountId { get; set; }
}

public class VsDbContext(DbContextOptions<VsDbContext> options) : DbContext(options)
{
    public DbSet<AccountRecord> Accounts => Set<AccountRecord>();
    public DbSet<TransactionRecord> Transactions => Set<TransactionRecord>();
}
