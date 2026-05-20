namespace ArchStudy.Ddd.Domain;

public sealed record Money(decimal Amount)
{
    public static Money Zero { get; } = new(0m);

    public bool IsPositive => Amount > 0m;

    public Money Plus(Money other) => new(Amount + other.Amount);
    public Money Minus(Money other) => new(Amount - other.Amount);
    public bool IsLessThan(Money other) => Amount < other.Amount;

    public static Money Of(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Money must be positive", nameof(amount));
        return new Money(amount);
    }
}
