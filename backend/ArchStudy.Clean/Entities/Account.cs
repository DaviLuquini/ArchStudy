namespace ArchStudy.Clean.Entities;

public class Account
{
    public Guid Id { get; private set; }
    public string Owner { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Account() { }

    public static Account Open(string owner, DateTime now) =>
        new() { Id = Guid.NewGuid(), Owner = owner, Balance = 0m, CreatedAt = now };

    public bool TryDeposit(decimal amount)
    {
        if (amount <= 0) return false;
        Balance += amount;
        return true;
    }

    public bool TryWithdraw(decimal amount)
    {
        if (amount <= 0 || Balance < amount) return false;
        Balance -= amount;
        return true;
    }
}
