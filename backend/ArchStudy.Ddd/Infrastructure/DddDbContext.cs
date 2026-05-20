using ArchStudy.Ddd.Domain;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Ddd.Infrastructure;

public class DddDbContext(DbContextOptions<DddDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(b =>
        {
            b.HasKey(a => a.Id);
            b.Property(a => a.Owner).IsRequired();
            b.OwnsOne(a => a.Balance, m => m.Property(p => p.Amount).HasColumnName("Balance"));
            b.Property(a => a.CreatedAt);
        });

        modelBuilder.Entity<Transaction>(b =>
        {
            b.HasKey(t => t.Id);
            b.Property(t => t.AccountId);
            b.Property(t => t.Type);
            b.OwnsOne(t => t.Amount, m => m.Property(p => p.Amount).HasColumnName("Amount"));
            b.Property(t => t.Timestamp);
            b.Property(t => t.RelatedAccountId);
        });
    }
}
