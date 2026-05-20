using ArchStudy.Clean.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Clean.Adapters.Persistence;

public class CleanDbContext(DbContextOptions<CleanDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(b =>
        {
            b.HasKey(a => a.Id);
            b.Property(a => a.Owner).IsRequired();
        });

        modelBuilder.Entity<Transaction>(b => b.HasKey(t => t.Id));
    }
}
