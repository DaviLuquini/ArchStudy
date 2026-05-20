using ArchStudy.Hexagonal.Domain;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Hexagonal.Adapters.Driven.Persistence;

public class HexagonalDbContext(DbContextOptions<HexagonalDbContext> options) : DbContext(options)
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
