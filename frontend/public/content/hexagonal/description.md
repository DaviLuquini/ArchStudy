> Domínio no centro. Tudo que entra ou sai passa por uma **porta** — uma interface. Quem implementa a porta é um **adaptador**.

## O que é

**Hexagonal Architecture** (Alistair Cockburn, 2005), ou *Ports & Adapters*, organiza o sistema em torno de um domínio isolado. Duas direções de portas:

- **Driving ports** (primárias) — definem o que o mundo externo pode pedir. Ex: `IAccountUseCases.DepositAsync`. Quem _dirige_ a aplicação são os driving adapters (HTTP, CLI, fila).
- **Driven ports** (secundárias) — definem o que o domínio precisa do mundo externo. Ex: `IAccountStore.LoadAsync`. Quem _é dirigido_ são os driven adapters (EF Core, Redis, S3).

## Como o request flui

1. `Adapters/Driving/Http/HexagonalEndpoints.cs` mapeia HTTP → `IAccountUseCases` (driving port).
2. `Application/AccountUseCases.cs` **implementa** o driving port. Carrega o domínio via `IAccountStore` (driven port), aplica a operação, registra a transação via `ITransactionStore`, commita via `ITransactionalScope`.
3. `Domain/Account.cs` valida internamente em `Credit` / `Debit` — invariantes ficam no agregado.
4. `Adapters/Driven/Persistence/EfStores.cs` implementa os driven ports com EF Core. Trocar SQLite por Postgres = outro adapter.

## Trade-offs

| Prós | Contras |
| --- | --- |
| **Trocar I/O é trivial** — outro `IAccountStore` em memória para testes | **Cerimônia alta** — 3-4 interfaces antes de salvar uma linha |
| Domínio dogmaticamente puro — não sabe nada além de C# | Confusão "Application × Domain" — onde fica cada regra? |
| Limites simétricos — driving e driven recebem o mesmo tratamento | Não resolve modelagem — dá pra ter domínio anêmico aqui também |

## Quando usar

Sistemas com **múltiplos canais** de entrada/saída — HTTP, CLI, filas. Quando trocar providers é frequente (fake stores em testes, banco diferente em prod e dev). Quando o domínio é o **ativo principal** e a infra é descartável.
