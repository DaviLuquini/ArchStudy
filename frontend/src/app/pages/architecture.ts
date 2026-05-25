import { ChangeDetectionStrategy, Component, computed, effect, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ARCHITECTURES, ContentService, findArchitecture } from '../data/architectures';
import { SeoService } from '../data/seo';
import { MermaidView } from '../components/mermaid-view';
import { MarkdownView } from '../components/markdown-view';
import { StructureSection } from '../components/structure-section';

@Component({
  selector: 'app-architecture',
  imports: [RouterLink, MermaidView, MarkdownView, StructureSection],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @let arch = architecture();
    @if (arch) {
      <article class="page" [attr.data-slug]="arch.slug">
        <header class="page-header">
          <nav class="breadcrumb" aria-label="Trilha">
            <a routerLink="/">Início</a>
            <span class="sep" aria-hidden="true">／</span>
            <span class="current">{{ categoryLabel(arch.category) }}</span>
          </nav>

          <p class="order-line">
            <span class="order-num">{{ orderLabel(arch.order) }}</span>
            <span class="order-sep" aria-hidden="true">·</span>
            <span class="order-cat">{{ categoryLabel(arch.category) }}</span>
          </p>
          <h1>{{ arch.name }}</h1>
          <p class="lead">{{ arch.tagline }}</p>

          <div class="nav-actions">
            @if (previous()) {
              <a class="nav-btn" [routerLink]="['/arch', previous()!.slug]" rel="prev">
                <span class="nav-arrow" aria-hidden="true">←</span>
                <span class="nav-meta">
                  <span class="nav-meta-label">Anterior</span>
                  <span class="nav-meta-name">{{ previous()!.name }}</span>
                </span>
              </a>
            } @else {
              <span class="nav-spacer" aria-hidden="true"></span>
            }
            @if (next()) {
              <a class="nav-btn nav-btn-right" [routerLink]="['/arch', next()!.slug]" rel="next">
                <span class="nav-meta">
                  <span class="nav-meta-label">Próxima</span>
                  <span class="nav-meta-name">{{ next()!.name }}</span>
                </span>
                <span class="nav-arrow" aria-hidden="true">→</span>
              </a>
            } @else {
              <span class="nav-spacer" aria-hidden="true"></span>
            }
          </div>
        </header>

        <section class="section section-diagram" aria-labelledby="diagram-heading">
          <div class="section-head">
            <h2 id="diagram-heading">Diagrama</h2>
            <span class="section-meta">Mermaid · gerado em runtime</span>
          </div>
          @if (diagram(); as src) {
            <app-mermaid-view [source]="src" [label]="'Diagrama da arquitetura ' + arch.name" />
          } @else if (loading()) {
            <p class="loading">Carregando diagrama…</p>
          } @else if (error()) {
            <p class="error" role="alert">Falha ao carregar conteúdo.</p>
          }
        </section>

        <section class="section" aria-labelledby="structure-heading">
          <div class="section-head">
            <h2 id="structure-heading">Estrutura</h2>
            <span class="section-meta">Pastas e arquivos do backend</span>
          </div>
          <app-structure-section [slug]="arch.slug" />
        </section>

        <section class="section" aria-labelledby="description-heading">
          <div class="section-head">
            <h2 id="description-heading">Sobre</h2>
            <span class="section-meta">Trade-offs e contexto</span>
          </div>
          @if (description(); as md) {
            <app-markdown-view [source]="md" />
          } @else if (loading()) {
            <p class="loading">Carregando descrição…</p>
          }
        </section>

        <footer class="page-footer">
          <a class="compare-link" routerLink="/compare" [queryParams]="{ a: arch.slug, b: nextOrPrevious()?.slug }">
            Comparar com {{ nextOrPrevious()?.name }}
            <span aria-hidden="true">→</span>
          </a>
        </footer>
      </article>
    } @else {
      <p class="not-found">
        Arquitetura não encontrada. <a routerLink="/">Voltar</a>.
      </p>
    }
  `,
  styles: [
    `
      :host {
        display: block;
        padding: 3rem 1.5rem 5rem;
        max-width: 900px;
        margin: 0 auto;
      }

      .page {
        animation: page-in 320ms var(--ease-out) both;
      }

      @keyframes page-in {
        from { opacity: 0; transform: translateY(6px); }
        to { opacity: 1; transform: translateY(0); }
      }

      .breadcrumb {
        display: flex;
        gap: 0.5rem;
        font-family: var(--font-sans);
        font-size: 0.82rem;
        color: var(--ink-muted);
        margin-bottom: 1.5rem;
        align-items: center;
      }

      .breadcrumb a {
        color: var(--ink-soft);
        text-decoration: none;
        transition: color 160ms var(--ease-out);
      }

      .breadcrumb a:hover { color: var(--accent); }

      .breadcrumb .sep { color: var(--ink-muted); font-size: 0.7rem; }
      .breadcrumb .current { color: var(--ink-soft); }

      .order-line {
        font-family: var(--font-mono);
        font-size: 0.75rem;
        text-transform: uppercase;
        letter-spacing: 0.1em;
        color: var(--ink-muted);
        margin: 0 0 0.5rem;
        display: inline-flex;
        align-items: center;
        gap: 0.5rem;
      }

      .order-num { color: var(--accent); font-weight: 600; }
      .order-sep { opacity: 0.4; }

      h1 {
        font-family: var(--font-display);
        font-size: clamp(2rem, 4vw, 2.85rem);
        margin: 0.2rem 0 0.8rem;
        color: var(--ink);
        font-weight: 700;
        line-height: 1.05;
        letter-spacing: -0.02em;
        font-variation-settings: 'opsz' 144, 'SOFT' 40;
      }

      .lead {
        font-family: var(--font-body);
        color: var(--ink-soft);
        font-size: 1.18rem;
        margin: 0;
        line-height: 1.5;
        font-style: italic;
        max-width: 60ch;
      }

      .nav-actions {
        display: flex;
        justify-content: space-between;
        gap: 0.75rem;
        margin: 2.2rem 0 1.5rem;
        flex-wrap: wrap;
      }

      .nav-spacer { flex: 1; }

      .nav-btn {
        flex: 1;
        max-width: 48%;
        padding: 0.75rem 1rem;
        border: 1px solid var(--rule);
        background: var(--sheet);
        border-radius: 0.6rem;
        color: var(--ink);
        text-decoration: none;
        font-family: var(--font-sans);
        font-size: 0.88rem;
        display: inline-flex;
        align-items: center;
        gap: 0.65rem;
        transition: transform 160ms var(--ease-out), border-color 200ms var(--ease-out),
          background 200ms var(--ease-out);
      }

      .nav-btn-right {
        justify-content: flex-end;
        text-align: right;
      }

      @media (hover: hover) and (pointer: fine) {
        .nav-btn:hover {
          border-color: var(--ink);
          background: var(--paper-deep);
        }
        .nav-btn:hover .nav-arrow { transform: translateX(3px); }
        .nav-btn-right:hover .nav-arrow { transform: translateX(3px); }
        .nav-btn:not(.nav-btn-right):hover .nav-arrow { transform: translateX(-3px); }
      }

      .nav-btn:active { transform: scale(0.98); }

      .nav-arrow {
        color: var(--accent);
        font-size: 1rem;
        transition: transform 220ms var(--ease-out);
      }

      .nav-meta {
        display: flex;
        flex-direction: column;
        gap: 0.1rem;
        min-width: 0;
      }

      .nav-meta-label {
        font-size: 0.7rem;
        text-transform: uppercase;
        letter-spacing: 0.1em;
        color: var(--ink-muted);
      }

      .nav-meta-name {
        font-weight: 500;
        color: var(--ink);
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }

      .section {
        background: var(--sheet);
        border: 1px solid var(--rule);
        border-radius: 0.95rem;
        padding: 1.6rem 1.8rem 2rem;
        margin-bottom: 1.5rem;
      }

      .section-head {
        display: flex;
        align-items: baseline;
        justify-content: space-between;
        gap: 1rem;
        flex-wrap: wrap;
        margin-bottom: 1.2rem;
        padding-bottom: 0.75rem;
        border-bottom: 1px dashed var(--rule);
      }

      .section h2 {
        font-family: var(--font-display);
        font-size: 1.25rem;
        font-style: italic;
        font-weight: 600;
        color: var(--ink);
        margin: 0;
        letter-spacing: -0.01em;
      }

      .section-meta {
        font-family: var(--font-mono);
        font-size: 0.72rem;
        color: var(--ink-muted);
        letter-spacing: 0.04em;
      }

      .loading { color: var(--ink-muted); font-style: italic; }
      .error { color: var(--accent); }

      .page-footer {
        margin-top: 2rem;
        padding: 1rem 1.25rem;
        background: var(--accent-tint);
        border: 1px solid var(--accent-soft);
        border-radius: 0.7rem;
      }

      .compare-link {
        display: inline-flex;
        align-items: center;
        gap: 0.5rem;
        color: var(--accent);
        font-family: var(--font-sans);
        font-weight: 500;
        text-decoration: none;
        font-size: 0.95rem;
        transition: gap 200ms var(--ease-out);
      }

      .compare-link:hover { gap: 0.85rem; }

      .not-found {
        padding: 2rem;
        text-align: center;
        font-family: var(--font-sans);
      }

      .not-found a { color: var(--accent); }
    `,
  ],
})
export class ArchitecturePage {
  readonly slug = input.required<string>();

  private readonly content = inject(ContentService);
  private readonly seo = inject(SeoService);

  protected readonly architecture = computed(() => findArchitecture(this.slug()));
  protected readonly loading = signal(true);
  protected readonly diagram = signal<string | null>(null);
  protected readonly description = signal<string | null>(null);
  protected readonly error = signal<string | null>(null);

  protected readonly previous = computed(() => {
    const arch = this.architecture();
    if (!arch) return null;
    return ARCHITECTURES.find((a) => a.order === arch.order - 1) ?? null;
  });

  protected readonly next = computed(() => {
    const arch = this.architecture();
    if (!arch) return null;
    return ARCHITECTURES.find((a) => a.order === arch.order + 1) ?? null;
  });

  protected readonly nextOrPrevious = computed(() => this.next() ?? this.previous());

  protected orderLabel(order: number): string {
    return `№ ${order.toString().padStart(2, '0')} / 08`;
  }

  protected categoryLabel(category: string): string {
    switch (category) {
      case 'camadas':
        return 'Camadas';
      case 'features':
        return 'Features';
      case 'dominio':
        return 'Domínio';
      default:
        return category;
    }
  }

  constructor() {
    effect(() => {
      const arch = this.architecture();
      if (!arch) {
        this.loading.set(false);
        return;
      }
      this.seo.update({
        title: `${arch.name} · ArchStudy`,
        description: arch.tagline,
        type: 'article',
      });
      this.loading.set(true);
      this.diagram.set(null);
      this.description.set(null);
      this.error.set(null);
      Promise.all([this.content.loadDiagram(arch.slug), this.content.loadDescription(arch.slug)])
        .then(([diagram, description]) => {
          this.diagram.set(diagram);
          this.description.set(description);
          this.loading.set(false);
        })
        .catch((err: unknown) => {
          this.error.set(err instanceof Error ? err.message : String(err));
          this.loading.set(false);
        });
    });
  }
}
