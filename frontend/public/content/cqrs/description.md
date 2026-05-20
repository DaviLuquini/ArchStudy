> Modificação e consulta são problemas diferentes. Cada lado pode ter modelo, persistência e escala próprios.

## O que é

**CQRS** — _Command Query Responsibility Segregation_ — separa **modificações de estado** (Commands) das **consultas** (Queries). Cada lado evolui independente: modelo diferente, contexto diferente, escala diferente.

> **Não é Event Sourcing.** Dá pra ter CQRS sem ES (este projeto) e ES sem CQRS. A confusão é comum, vale repetir.

## Como o request flui

1. **POSTs** vão para `ICommandHandler<TCmd, TResp>`. `Commands/Deposit.cs`, `Withdraw.cs`, `Transfer.cs`, `CreateAccount.cs` — cada um um record + handler. Escrevem via `CqrsWriteDbContext` (com change tracking).
2. **GETs** vão para `IQueryHandler<TQ, TResp>`. `Queries/GetAccount.cs`, `GetStatement.cs` — record + handler. Lêem via `CqrsReadDbContext` com `.AsNoTracking()` e **projetam direto para o DTO** de resposta — zero entidades em memória.
3. Endpoints injetam `ICommandHandler<...>` ou `IQueryHandler<...>` — a separação aparece no **tipo**.
4. Aqui ambos os contextos apontam para o mesmo SQLite. Em sistemas reais, write em Postgres + read em ElasticSearch é típico — com sincronização em background.

```text
backend/ArchStudy.Cqrs/
├── Commands/                       ← write side
│   ├── CreateAccount.cs
│   ├── Deposit.cs / Withdraw.cs / Transfer.cs
│   └── (record : ICommand + ICommandHandler)
├── Queries/                        ← read side
│   ├── GetAccount.cs
│   ├── GetStatement.cs
│   └── (record : IQuery + IQueryHandler)
├── Domain/WriteModel.cs            ← entidades de escrita
├── Infrastructure/
│   ├── CqrsWriteDbContext.cs       ← change tracking
│   └── CqrsReadDbContext.cs        ← AsNoTracking
└── Presentation/CqrsEndpoints.cs
```

## Trade-offs

| Prós | Contras |
| --- | --- |
| **Modelos otimizados por uso** — query projeta direto pro DTO | **Eventual consistency** assim que os modelos divergem fisicamente |
| Escala **assimétrica** — read replicas sem mexer no write | Pouco ganho em CRUDs simples — só atrapalha |
| Linguagem expressiva — "qual command?" / "qual query?" | Sincronização entre stores é problema próprio (retry, mensageria) |
| Casa bem com Vertical Slice — cada command/query vira um arquivo | **Confunde com Event Sourcing** — leitura cuidadosa exigida |

## Quando usar

Sistemas com **carga assimétrica** (muito mais leitura que escrita, ou vice-versa). Domínios em que o modelo para escrever e o modelo para mostrar são **genuinamente diferentes** — catálogo de produtos vs. relatório de vendas. Microsserviços onde write e read podem ser deployados independentes.
