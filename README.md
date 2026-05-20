# ArchStudy

Estudo visual de arquiteturas de software: **o mesmo domínio (conta bancária) implementado em 8 arquiteturas diferentes em .NET 10**, com um visualizador Angular que mostra diagramas e descrições lado a lado.

Cada uma das 8 arquiteturas resolve o mesmo problema (criar conta, depositar, sacar, transferir, consultar saldo, ver extrato) com o mesmo contrato HTTP. A única variável é a forma de organizar o código.

## Arquiteturas

| # | Slug | Pasta |
|---|------|-------|
| 1 | MVC + Services + Repositories | `backend/ArchStudy.Mvc/` |
| 2 | Feature-Based | `backend/ArchStudy.FeatureBased/` |
| 3 | Vertical Slice | `backend/ArchStudy.VerticalSlice/` |
| 4 | Onion Architecture | `backend/ArchStudy.Onion/` |
| 5 | Hexagonal (Ports & Adapters) | `backend/ArchStudy.Hexagonal/` |
| 6 | Clean Architecture | `backend/ArchStudy.Clean/` |
| 7 | Domain-Driven Design (tático) | `backend/ArchStudy.Ddd/` |
| 8 | CQRS | `backend/ArchStudy.Cqrs/` |

## Estrutura

```
ArchStudy/
├── backend/
│   ├── ArchStudy.slnx            # Solution .NET; agrupa os 10 projetos
│   ├── ArchStudy.Host/           # Web API único; registra os 8 módulos sob prefixos
│   ├── ArchStudy.Shared/         # DTOs comuns (request/response)
│   └── ArchStudy.<Arq>/          # 8 projetos de biblioteca, um por arquitetura
└── frontend/                     # Angular 21 + Tailwind + Mermaid (visualizador)
```

## API contract (igual em todas as 8 arquiteturas)

Substitua `{arq}` por `mvc`, `feature-based`, `vertical-slice`, `onion`, `hexagonal`, `clean`, `ddd` ou `cqrs`.

- `POST   /{arq}/accounts` — criar conta `{ "owner": "Alice" }`
- `GET    /{arq}/accounts/{id}` — consultar saldo
- `GET    /{arq}/accounts/{id}/transactions` — extrato
- `POST   /{arq}/accounts/{id}/deposits` — depositar `{ "amount": 100 }`
- `POST   /{arq}/accounts/{id}/withdrawals` — sacar `{ "amount": 30 }`
- `POST   /{arq}/transfers` — transferir `{ "fromAccountId": "…", "toAccountId": "…", "amount": 20 }`

Cada módulo persiste num arquivo SQLite isolado (`archstudy-mvc.db`, `archstudy-hexagonal.db`, …) criado na primeira execução.

## Como rodar

### Backend (.NET 10)

```bash
dotnet run --project backend/ArchStudy.Host
```

Sobe em `http://localhost:5129`. OpenAPI em `http://localhost:5129/openapi/v1.json`.

### Frontend (Angular 21)

```bash
cd frontend
npm install
npm start
```

Abra `http://localhost:4200`.

## Visualizador

- **Home (`/`)**: catálogo das 8 arquiteturas.
- **Arquitetura (`/arch/:slug`)**: diagrama Mermaid + descrição em markdown (prós, contras, quando usar).
- **Comparar (`/compare?a=mvc&b=ddd`)**: split lado a lado.

O conteúdo educacional fica em `frontend/public/content/{slug}/` (`diagram.mmd` + `description.md`) — editar não exige rebuild.
