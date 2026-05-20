namespace ArchStudy.Ddd.Domain;

public class Account
{
    public Guid Id { get; private set; }
    public string Owner { get; private set; } = string.Empty;
    public Money Balance { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Account() { }

    public static Account Open(string owner)
    {
        if (string.IsNullOrWhiteSpace(owner))
            throw new ArgumentException("Owner is required", nameof(owner));

        return new Account
        {
            Id = Guid.NewGuid(),
            Owner = owner,
            Balance = Money.Zero,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void Deposit(Money amount)
    {
        if (!amount.IsPositive) throw new ArgumentException("Amount must be positive");
        Balance = Balance.Plus(amount);
    }

    public void Withdraw(Money amount)
    {
        if (!amount.IsPositive) throw new ArgumentException("Amount must be positive");
        if (Balance.IsLessThan(amount)) throw new InsufficientFundsException(Id);
        Balance = Balance.Minus(amount);
    }
}

public sealed class InsufficientFundsException(Guid accountId)
    : InvalidOperationException($"Account {accountId} has insufficient funds");
