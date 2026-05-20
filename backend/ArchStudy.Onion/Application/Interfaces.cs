using ArchStudy.Onion.Domain;

namespace ArchStudy.Onion.Application;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Account account, CancellationToken ct);
}

public interface ITransactionRepository
{
    Task<IReadOnlyList<Transaction>> ListByAccountAsync(Guid accountId, CancellationToken ct);
    Task AddAsync(Transaction transaction, CancellationToken ct);
}

public interface IUnitOfWork
{
    Task SaveAsync(CancellationToken ct);
}
