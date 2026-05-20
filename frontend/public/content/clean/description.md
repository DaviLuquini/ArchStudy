> Cada caso de uso é uma classe. Erros viram `Result<T>`. Exceções? Só para bug, nunca para fluxo.

## O que é

**Clean Architecture** (Robert C. Martin, 2012) sistematiza Onion / Hexagonal com uma regra adicional: **um caso de uso = uma classe**. `CreateAccountUseCase`, `DepositUseCase`, `WithdrawUseCase`, `TransferUseCase`… cada um isolado.

E uma segunda regra que diferencia da Onion: **erros são valores, não exceções**. `UseCaseResult<T>` força o endpoint a tratar cada caminho.

## Como o request flui

1. `Entities/Account.cs` valida via `TryDeposit` / `TryWithdraw` que retornam `bool`. Sem exceção.
2. `UseCases/Deposit.cs` carrega o gateway, chama a entidade, registra transação. Retorna `UseCaseResult<AccountResponse>.Ok | NotFound | Invalid`.
3. `Frameworks/Web/CleanEndpoints.cs` pattern-matcheia o resultado para `Results.Ok`, `Results.NotFound` ou `Results.BadRequest`. O HTTP é tradução, não decisão.
4. `Adapters/Persistence/EfGateways.cs` implementa `IAccountGateway`, `ITransactionGateway`, `IPersistenceContext`.

```text
backend/ArchStudy.Clean/
├── Entities/                            ← centro
│   ├── Account.cs                       ← TryDeposit/TryWithdraw → bool
│   └── Transaction.cs
├── UseCases/                            ← uma classe por caso de uso
│   ├── CreateAccount.cs
│   ├── Deposit.cs / Withdraw.cs / Transfer.cs
│   ├── GetAccount.cs / GetStatement.cs
│   └── Common/Result.cs                 ← UseCaseResult<T> = Ok | NotFound | Invalid
├── Adapters/Persistence/                ← EF Core gateways
└── Frameworks/Web/CleanEndpoints.cs     ← traduz Result em HTTP
```

## Trade-offs

| Prós | Contras |
| --- | --- |
| **Casos de uso encapsulados** — cada um sabe o que precisa e faz uma coisa | **Mais arquivos ainda** que Onion |
| Erros **explícitos** — `Result<T>` força tratar cada caminho | Endpoints viram `switch`-stories ao mapear resultado para HTTP |
| Testes diretos — `new DepositUseCase(fakeGateway, fakeContext)` | DTOs viram quatro: request, input do use case, entidade, response |

## Quando usar

Sistemas críticos onde **cada caminho de erro precisa ser explícito**. Times que querem revisões de PR objetivas guiadas por convenção. Quando o **domínio é o ativo central** e a expectativa de longevidade do código é alta.
