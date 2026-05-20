> Mesmas camadas do MVC, mas a pasta vira sobre **comportamento** — não sobre mecânica. Tudo de "Depósito" mora junto.

## O que é

**Feature-Based** mantém o trio Controller / Service / Repository, mas troca a organização das pastas. Em vez de `Controllers/`, `Services/`, `Repositories/` na raiz, há uma pasta por feature: `Features/Deposits`, `Features/Withdrawals`, `Features/Transfers`…

Dentro de cada pasta ficam **os três arquivos** daquela feature — um controller, um service e um repository. Mexer em depósito não obriga a abrir mais nada.

## Como o request flui

1. Cada pasta de feature tem **três arquivos**: `XxxController.cs` mapeia os endpoints, `XxxService.cs` aplica as regras de negócio e `XxxRepository.cs` conversa com o banco.
2. O fluxo dentro da feature é o mesmo do MVC — controller → service → repository — só que as três peças ficam lado a lado em `Features/Deposits/`, e não espalhadas por pastas técnicas.
3. Cada feature tem seu próprio `IXxxRepository`. Sim, há duplicação. Sim, isso é proposital.
4. Um único `FeatureBasedDbContext` mora em `Shared/` porque a tabela `Accounts` é referenciada por todas as features.
5. Erros e validação são iguais ao MVC: exceções convertidas no middleware.

```text
backend/ArchStudy.FeatureBased/
├── Features/
│   ├── Accounts/
│   │   ├── AccountsController.cs    ← endpoints: criar e consultar
│   │   ├── AccountsService.cs       ← regras de negócio
│   │   └── AccountsRepository.cs    ← acesso a dados
│   ├── Deposits/
│   │   ├── DepositsController.cs
│   │   ├── DepositsService.cs
│   │   └── DepositsRepository.cs
│   ├── Withdrawals/                 ← mesma tríade Controller/Service/Repository
│   ├── Transfers/                   ← mesma tríade
│   └── Statements/                  ← mesma tríade
└── Shared/
    └── FeatureBasedDbContext.cs     ← DbContext compartilhado
```

## Trade-offs

| Prós | Contras |
| --- | --- |
| **Coesão alta** — mudou a regra de depósito, mexe em uma pasta só | **Duplicação honesta** — vários `FindAccountAsync` parecidos espalhados |
| **Onboarding** — devs novos exploram o sistema por comportamento | Compartilhamentos viram dor: a pasta `Shared/` cresce em discussão |
| **PRs mais focados** — menos merge conflicts entre squads | Ainda **anêmico** — a entidade segue como saco de dados |

## Quando usar

Sistemas grandes em que features evoluem em **ritmos diferentes**. Times distribuídos com ownership por feature. Quando você quer reduzir conflitos de merge entre squads — e topa repetir um pouco de código a troco disso.
