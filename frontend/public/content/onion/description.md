> Camadas concêntricas e uma regra inflexível: **dependências só apontam para dentro**. O domínio mora no centro e não sabe que EF existe.

## O que é

**Onion Architecture** (Jeffrey Palermo, 2008) organiza o sistema em camadas concêntricas. O **domínio** fica no centro, sem dependência externa. Em volta, **Application** define interfaces e usa o domínio. Mais fora, **Infrastructure** implementa as interfaces. Por último, **Presentation** fala com Application.

A regra "saldo não pode ficar negativo" mora **dentro de `Account.Withdraw`**, não no service. A entidade protege seus próprios invariantes — e ninguém de fora consegue burlar.

## Como o request flui

1. `Presentation/OnionEndpoints.cs` mapeia HTTP → `AccountAppService`.
2. `Application/AccountAppService.cs` carrega o agregado via `IAccountRepository`, chama métodos de domínio, registra a transação, salva via `IUnitOfWork`.
3. `Domain/Account.cs` valida internamente em `Deposit` / `Withdraw`. Setters privados, instanciação via `Account.Create()`.
4. `Infrastructure/AccountRepository.cs` implementa `IAccountRepository` via EF Core — invisível para o domínio.

```text
backend/ArchStudy.Onion/
├── Domain/                  ← núcleo, sem dependências
│   ├── Account.cs           ← Deposit / Withdraw com validação
│   └── Transaction.cs
├── Application/             ← contratos + orquestração
│   ├── Interfaces.cs        ← IAccountRepository, IUnitOfWork...
│   └── AccountAppService.cs
├── Infrastructure/          ← implementações EF Core
│   ├── OnionDbContext.cs
│   └── Repositories.cs
└── Presentation/OnionEndpoints.cs
```

## Trade-offs

| Prós | Contras |
| --- | --- |
| **Domínio puro** — `Account` viaja para qualquer projeto, qualquer banco | **Mais arquivos** — 3 do MVC viram 6 aqui |
| Inversão de dependência **explícita** — código segue a teoria | Confusão com Hexagonal e Clean — três variações da mesma ideia |
| Fácil de testar — `IAccountRepository` em memória, sem subir EF | Sem disciplina, `AccountAppService` vira o velho *manager* |

## Quando usar

Sistemas com regras de negócio **razoavelmente ricas**. Times que querem manter o domínio independente de framework e banco. Quando você antecipa **trocar provider** — por exemplo, migrar EF Core para Dapper, ou SQLite para Postgres.
