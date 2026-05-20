> O fluxo mais reconhecível em ASP.NET. Controller pede, Service decide, Repository busca — e a entidade só carrega dados.

## O que é

**MVC + Services + Repositories** é a organização **N-tier clássica**. O fluxo é linear e fácil de seguir: o endpoint recebe a request, valida superficialmente, delega para um *service* que carrega o estado via *repository* e aplica as regras sobre uma entidade **anêmica** — só propriedades, sem comportamento.

A regra "saldo não pode ficar negativo" vive **no service**, longe da entidade. É a marca registrada do estilo, e também a sua principal limitação.

## Como o request flui

1. `AccountsController` mapeia rotas sob `/mvc` e repassa para os services.
2. `AccountService` carrega o `AccountEntity`, valida `amount > 0`, valida saldo, muta o objeto e dispara `SaveChangesAsync`.
3. `TransferService` orquestra duas contas dentro da mesma unidade de trabalho.
4. Erros viram **exceções** — `ArgumentException`, `InvalidOperationException`, `KeyNotFoundException` — mapeadas para HTTP no `ExceptionMappingMiddleware`.

```text
backend/ArchStudy.Mvc/
├── Controllers/AccountsController.cs   ← endpoints
├── Services/AccountService.cs          ← regras
├── Services/TransferService.cs
├── Repositories/AccountRepository.cs   ← acesso a dados
├── Repositories/TransactionRepository.cs
└── Persistence/MvcDbContext.cs         ← EF Core + entidades anêmicas
```

## Trade-offs

| Prós | Contras |
| --- | --- |
| **Familiar** — quase todo dev .NET reconhece de cara | **Entidade anêmica** — a regra de saldo vive no service, não na conta |
| **Baixa curva** — não precisa ler livro nenhum | Services viram *managers* de mil linhas em projetos grandes |
| **Pouca cerimônia** — adicionar um campo demanda poucos arquivos | Difícil testar a regra **sem subir EF** |

## Quando usar

Projetos **pequenos**, CRUDs com pouca regra de negócio, equipes acostumadas com .NET tradicional, prototipagem rápida. Quando você está validando uma ideia e quer chegar no banco em 30 minutos.
