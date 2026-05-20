using ArchStudy.Onion.Domain;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Onion.Infrastructure;

public class OnionDbContext(DbContextOptions<OnionDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(b =>
        {
            b.HasKey(a => a.Id);
            b.Property(a => a.Owner).IsRequired();
            b.Property(a => a.Balance);
            b.Property(a => a.CreatedAt);
        });

        modelBuilder.Entity<Transaction>(b =>
        {
            b.HasKey(t => t.Id);
            b.Property(t => t.AccountId);
            b.Property(t => t.Type);
            b.Property(t => t.Amount);
            b.Property(t => t.Timestamp);
            b.Property(t => t.RelatedAccountId);
        });
    }
}
