> O domínio deixa de ser anêmico. `Account.Withdraw(Money.Of(100))` lê como o time fala. Impossível bypassar a regra.

## O que é

**DDD** (Eric Evans, 2003) é mais filosofia que arquitetura. Tem dois recortes:

- **Estratégico** — Bounded Contexts, linguagem ubíqua, mapas de contexto.
- **Tático** — Aggregate, Aggregate Root, Value Object, Domain Service, Repository.

Este estudo foca no **DDD tático** — o que muda no código quando você modela o domínio com **comportamento** em vez de só dados.

## Os conceitos no código

1. **`Money`** é um *Value Object*. Imutável, comparado por valor, com operações próprias (`Plus`, `Minus`, `IsLessThan`). `Money.Of(0)` falha — o tipo carrega a regra.
2. **`Account`** é um *Aggregate Root*. Sem setter público. Criado via `Account.Open(owner)`. Os métodos `Deposit(Money)` / `Withdraw(Money)` são a **única forma** de mudar o saldo, e validam invariantes internamente — `InsufficientFundsException` nasce dentro do agregado.
3. **`TransferDomainService`** é um *Domain Service*. Existe porque transferir **atravessa dois agregados** e não pertence naturalmente a nenhum.
4. **`IAccountRepository`** retorna o agregado **completo**. Não DTO. A `AccountApplicationService` só conhece o agregado.

```csharp
// O agregado protege seus próprios invariantes
public void Withdraw(Money amount)
{
    if (!amount.IsPositive) throw new ArgumentException(...);
    if (Balance.IsLessThan(amount)) throw new InsufficientFundsException(Id);
    Balance = Balance.Minus(amount);
}
```

## Trade-offs

| Prós | Contras |
| --- | --- |
| **Regras vivem no agregado** — impossível um caller bypassar | **Curva de aprendizado** — Aggregate, VO, Domain Service: vocabulário denso |
| Modelo expressivo — `account.Deposit(Money.Of(100))` | Modelagem demora — discussões sobre "onde fica essa regra" |
| Testes de domínio **super baratos** — sem mocks, sem EF | Persistência de Value Objects pede mapeamento (`OwnsOne`) |
| Refatoração interna do agregado **não vaza** | Overhead gratuito para CRUDs simples |

## Quando usar

Quando o domínio é **complexo e cheio de regras** — banking, seguros, logística. Quando o time tem espaço para conversar com o negócio e refinar a linguagem. Projetos **longos** onde a clareza do modelo paga juros compostos.
