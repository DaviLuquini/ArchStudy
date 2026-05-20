using ArchStudy.Hexagonal.Domain;

namespace ArchStudy.Hexagonal.Ports.Driven;

public interface IAccountStore
{
    Task<Account?> LoadAsync(Guid id, CancellationToken ct);
    Task SaveAsync(Account account, CancellationToken ct);
}

public interface ITransactionStore
{
    Task<IReadOnlyList<Transaction>> ListByAccountAsync(Guid accountId, CancellationToken ct);
    Task RecordAsync(Transaction transaction, CancellationToken ct);
}

public interface ITransactionalScope
{
    Task CommitAsync(CancellationToken ct);
}
