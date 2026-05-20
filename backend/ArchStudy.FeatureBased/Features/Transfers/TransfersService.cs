using ArchStudy.FeatureBased.Shared;
using ArchStudy.Shared.Dtos;

namespace ArchStudy.FeatureBased.Features.Transfers;

public interface ITransfersService
{
    Task TransferAsync(Guid fromId, Guid toId, decimal amount, CancellationToken ct);
}

public class TransfersService(ITransfersRepository repo) : ITransfersService
{
    public async Task TransferAsync(Guid fromId, Guid toId, decimal amount, CancellationToken ct)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive");
        if (fromId == toId) throw new ArgumentException("Accounts must differ");

        var from = await repo.FindAccountAsync(fromId, ct) ?? throw new KeyNotFoundException("Source account not found");
        var to = await repo.FindAccountAsync(toId, ct) ?? throw new KeyNotFoundException("Destination account not found");
        if (from.Balance < amount) throw new InvalidOperationException("Insufficient funds");

        from.Balance -= amount;
        to.Balance += amount;
        var now = DateTime.UtcNow;
        await repo.AddTransactionAsync(new TransactionRow { Id = Guid.NewGuid(), AccountId = from.Id, Type = TransactionType.TransferOut, Amount = amount, Timestamp = now, RelatedAccountId = to.Id }, ct);
        await repo.AddTransactionAsync(new TransactionRow { Id = Guid.NewGuid(), AccountId = to.Id, Type = TransactionType.TransferIn, Amount = amount, Timestamp = now, RelatedAccountId = from.Id }, ct);
        await repo.SaveAsync(ct);
    }
}
