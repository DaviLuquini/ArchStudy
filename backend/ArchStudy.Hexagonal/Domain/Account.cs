namespace ArchStudy.Hexagonal.Domain;

public class Account
{
    public Guid Id { get; private set; }
    public string Owner { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Account() { }

    public static Account Open(string owner) =>
        new()
        {
            Id = Guid.NewGuid(),
            Owner = owner,
            Balance = 0m,
            CreatedAt = DateTime.UtcNow,
        };

    public void Credit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive");
        Balance += amount;
    }

    public void Debit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive");
        if (Balance < amount) throw new InvalidOperationException("Insufficient funds");
        Balance -= amount;
    }
}
