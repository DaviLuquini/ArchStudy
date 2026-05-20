using ArchStudy.Mvc.Persistence;
using ArchStudy.Mvc.Repositories;
using ArchStudy.Shared.Dtos;

namespace ArchStudy.Mvc.Services;

public interface ITransferService
{
    Task TransferAsync(Guid fromId, Guid toId, decimal amount, CancellationToken ct);
}

public class TransferService(
    IAccountRepository accounts,
    ITransactionRepository transactions) : ITransferService
{
    public async Task TransferAsync(Guid fromId, Guid toId, decimal amount, CancellationToken ct)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive", nameof(amount));
        if (fromId == toId) throw new ArgumentException("Accounts must differ");

        var from = await accounts.GetByIdAsync(fromId, ct) ?? throw new KeyNotFoundException("Source account not found");
        var to = await accounts.GetByIdAsync(toId, ct) ?? throw new KeyNotFoundException("Destination account not found");
        if (from.Balance < amount) throw new InvalidOperationException("Insufficient funds");

        from.Balance -= amount;
        to.Balance += amount;

        var now = DateTime.UtcNow;
        await transactions.AddAsync(new TransactionEntity
        {
            Id = Guid.NewGuid(),
            AccountId = from.Id,
            Type = TransactionType.TransferOut,
            Amount = amount,
            Timestamp = now,
            RelatedAccountId = to.Id,
        }, ct);
        await transactions.AddAsync(new TransactionEntity
        {
            Id = Guid.NewGuid(),
            AccountId = to.Id,
            Type = TransactionType.TransferIn,
            Amount = amount,
            Timestamp = now,
            RelatedAccountId = from.Id,
        }, ct);

        await accounts.SaveChangesAsync(ct);
    }
}
