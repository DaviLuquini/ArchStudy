> Um caso de uso, **um handler**, um arquivo. Sem service. Sem repository. Sem cerimônia entre o request e o banco.

## O que é

**Vertical Slice** radicaliza o feature-based: cada caso de uso é *um único handler*. Nada de service, nada de repository — o handler recebe a request, valida, conversa com o `DbContext` direto, devolve a resposta.

A premissa é desconfortável: **duplicação é melhor que abstração errada**. Quatro handlers vão buscar conta por ID com a mesma linha — e tudo bem.

## Como o request flui

1. `Features/Deposit.cs` contém **tudo**: o `DepositCommand` (record), o `DepositHandler : IRequestHandler<DepositCommand, AccountResponse>` e o mapeamento HTTP. Cerca de 30 linhas.
2. Cada handler é registrado individualmente em `VerticalSliceModule.cs` — `AddScoped<IRequestHandler<DepositCommand, ...>, DepositHandler>`.
3. O endpoint injeta `IRequestHandler<TRequest, TResponse>` direto. Sem `IMediator`, sem indireção.
4. O `VsDbContext` é usado **dentro** do handler. Não há repository.

## Trade-offs

| Prós | Contras |
| --- | --- |
| **Coesão máxima** — 1 arquivo para entender 1 caso de uso | **Duplicação real** — `FirstOrDefaultAsync` e `amount > 0` repetidos |
| Cada slice **evolui isolado** — risco de regressão localizado | Sem "service compartilhado" — reusar lógica em batch obriga a duplicar ou refatorar |
| **Testável** — mocka só o `DbContext` | Confunde com "usar MediatR" — o estilo é a separação, não a ferramenta |

## Quando usar

APIs com **muitos casos de uso pequenos e independentes**. Equipes grandes onde o custo de coordenação supera o de duplicação. Sistemas que **crescem por adição** de features novas, não por modificação das existentes.
