namespace ArchStudy.Ddd.Domain;

public class TransferDomainService
{
    public void Transfer(Account from, Account to, Money amount)
    {
        if (from.Id == to.Id) throw new ArgumentException("Accounts must differ");
        from.Withdraw(amount);
        to.Deposit(amount);
    }
}
