namespace ArchStudy.Onion.Domain;

public class Account
{
    public Guid Id { get; private set; }
    public string Owner { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Account() { }

    public static Account Create(string owner) =>
        new()
        {
            Id = Guid.NewGuid(),
            Owner = owner,
            Balance = 0m,
            CreatedAt = DateTime.UtcNow,
        };

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive");
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive");
        if (Balance < amount) throw new InvalidOperationException("Insufficient funds");
        Balance -= amount;
    }
}
