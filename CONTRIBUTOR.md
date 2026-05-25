# Contributor Guide

Obrigado por considerar contribuir com o `ArchStudy`.

Este projeto compara o mesmo domínio de negócio implementado em múltiplas arquiteturas .NET e exposto por um visualizador em Angular. A ideia é manter o estudo consistente: o domínio e o contrato HTTP permanecem os mesmos, e a diferença principal está na organização interna de cada arquitetura.

## Como contribuir

Você pode contribuir de algumas formas:

- corrigindo bugs;
- melhorando a documentação;
- ajustando diagramas e descrições das arquiteturas;
- refinando a interface do visualizador;
- adicionando observações sobre trade-offs e estrutura de código.

## Setup local

### Backend

```bash
dotnet run --project backend/ArchStudy.Host
```

API disponível em `http://localhost:5129`.

### Frontend

```bash
cd frontend
npm install
npm start
```

Aplicação disponível em `http://localhost:4200`.

## Diretrizes

- Preserve o mesmo contrato HTTP entre as arquiteturas sempre que a mudança não for intencional.
- Mantenha o mesmo domínio de negócio em todas as implementações para facilitar a comparação.
- Prefira mudanças pequenas, focadas e fáceis de revisar.
- Ao alterar conteúdo educacional, tente deixar claros os trade-offs, não apenas os benefícios.
- Evite mudanças que descaracterizem o objetivo do projeto como material comparativo.

## Conteúdo educacional

Os materiais de cada arquitetura ficam em `frontend/public/content/{slug}/`.

Em geral, cada arquitetura possui:

- `description.md`: explicação, prós, contras e quando usar;
- `diagram.mmd`: diagrama Mermaid;
- `files.json`: estrutura de arquivos exibida no visualizador.

## Antes de abrir PR

- Verifique se o projeto roda localmente.
- Revise a clareza da documentação alterada.
- Se possível, teste o backend e o frontend após a mudança.
- Descreva no PR o que foi alterado e por quê.

## Sugestão de fluxo

1. Faça um fork do projeto.
2. Crie uma branch para sua alteração.
3. Implemente a mudança.
4. Abra um Pull Request com uma descrição objetiva.

Contribuições pequenas e bem explicadas costumam ser as melhores para revisão.
