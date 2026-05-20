using ArchStudy.Clean.Entities;

namespace ArchStudy.Clean.UseCases.Common;

public interface IAccountGateway
{
    Task<Account?> FindAsync(Guid id, CancellationToken ct);
    Task SaveAsync(Account account, CancellationToken ct);
}

public interface ITransactionGateway
{
    Task<IReadOnlyList<Transaction>> ListAsync(Guid accountId, CancellationToken ct);
    Task RecordAsync(Transaction transaction, CancellationToken ct);
}

public interface IPersistenceContext
{
    Task CommitAsync(CancellationToken ct);
}
