namespace ArchStudy.Ddd.Domain;

public interface IAccountRepository
{
    Task<Account?> LoadAsync(Guid id, CancellationToken ct);
    Task AddAsync(Account account, CancellationToken ct);
}

public interface ITransactionLog
{
    Task<IReadOnlyList<Transaction>> ListByAccountAsync(Guid accountId, CancellationToken ct);
    Task AppendAsync(Transaction transaction, CancellationToken ct);
}

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken ct);
}
