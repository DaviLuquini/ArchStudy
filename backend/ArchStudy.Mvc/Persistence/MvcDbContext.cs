using ArchStudy.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Mvc.Persistence;

public class AccountEntity
{
    public Guid Id { get; set; }
    public string Owner { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TransactionEntity
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid? RelatedAccountId { get; set; }
}

public class MvcDbContext(DbContextOptions<MvcDbContext> options) : DbContext(options)
{
    public DbSet<AccountEntity> Accounts => Set<AccountEntity>();
    public DbSet<TransactionEntity> Transactions => Set<TransactionEntity>();
}
