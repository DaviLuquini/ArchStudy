using ArchStudy.Cqrs.Domain;
using Microsoft.EntityFrameworkCore;

namespace ArchStudy.Cqrs.Infrastructure;

public class CqrsWriteDbContext(DbContextOptions<CqrsWriteDbContext> options) : DbContext(options)
{
    public DbSet<AccountWriteModel> Accounts => Set<AccountWriteModel>();
    public DbSet<TransactionWriteModel> Transactions => Set<TransactionWriteModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountWriteModel>(b =>
        {
            b.ToTable("Accounts");
            b.HasKey(a => a.Id);
        });
        modelBuilder.Entity<TransactionWriteModel>(b =>
        {
            b.ToTable("Transactions");
            b.HasKey(t => t.Id);
        });
    }
}

public class CqrsReadDbContext(DbContextOptions<CqrsReadDbContext> options) : DbContext(options)
{
    public DbSet<AccountWriteModel> Accounts => Set<AccountWriteModel>();
    public DbSet<TransactionWriteModel> Transactions => Set<TransactionWriteModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountWriteModel>(b =>
        {
            b.ToTable("Accounts");
            b.HasKey(a => a.Id);
        });
        modelBuilder.Entity<TransactionWriteModel>(b =>
        {
            b.ToTable("Transactions");
            b.HasKey(t => t.Id);
        });
    }
}
