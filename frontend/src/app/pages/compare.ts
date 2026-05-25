import { ChangeDetectionStrategy, Component, computed, effect, inject, input, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ARCHITECTURES, ContentService, findArchitecture } from '../data/architectures';
import { SeoService } from '../data/seo';
import { ArchSelector } from '../components/arch-selector';
import { MermaidView } from '../components/mermaid-view';
import { MarkdownView } from '../components/markdown-view';

interface Side {
  slug: string;
  diagram: string | null;
  description: string | null;
  loading: boolean;
}

@Component({
  selector: 'app-compare',
  imports: [RouterLink, ArchSelector, MermaidView, MarkdownView],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <article class="page">
      <header class="page-header">
        <nav class="breadcrumb" aria-label="Trilha">
          <a routerLink="/">Início</a>
          <span class="sep" aria-hidden="true">／</span>
          <span class="current">Comparar</span>
        </nav>
        <p class="order-line">Modo lado a lado</p>
        <h1>Duas arquiteturas, <em>uma tela</em></h1>
        <p class="lead">
          Escolha duas para ver diagrama e descrição lado a lado. Bom para enxergar trade-offs em par
          — onde uma ganha em simplicidade, a outra ganha em testabilidade.
        </p>
      </header>

      <div class="selectors">
        <app-arch-selector
          label="Lado esquerdo"
          [value]="leftSlug()"
          [exclude]="rightSlug()"
          (valueChange)="setLeft($event)"
        />
        <span class="vs" aria-hidden="true">vs</span>
        <app-arch-selector
          label="Lado direito"
          [value]="rightSlug()"
          [exclude]="leftSlug()"
          (valueChange)="setRight($event)"
        />
      </div>

      <div class="split">
        <section class="panel" aria-labelledby="left-title">
          <header class="panel-head">
            <span class="panel-tag">Esquerda</span>
            <h2 id="left-title">{{ leftArch()?.name }}</h2>
          </header>
          <div class="panel-body" [class.is-transitioning]="left().loading">
            @if (left().diagram; as src) {
              <app-mermaid-view [source]="src" [label]="leftArch()?.name ?? ''" />
            }
            @if (left().description; as md) {
              <app-markdown-view [source]="md" />
            }
          </div>
        </section>
        <section class="panel" aria-labelledby="right-title">
          <header class="panel-head">
            <span class="panel-tag">Direita</span>
            <h2 id="right-title">{{ rightArch()?.name }}</h2>
          </header>
          <div class="panel-body" [class.is-transitioning]="right().loading">
            @if (right().diagram; as src) {
              <app-mermaid-view [source]="src" [label]="rightArch()?.name ?? ''" />
            }
            @if (right().description; as md) {
              <app-markdown-view [source]="md" />
            }
          </div>
        </section>
      </div>
    </article>
  `,
  styles: [
    `
      :host {
        display: block;
        padding: 3rem 1.5rem 5rem;
        max-width: 1280px;
        margin: 0 auto;
      }

      .page { animation: page-in 320ms var(--ease-out) both; }

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
        color: var(--accent);
        margin: 0 0 0.5rem;
        font-weight: 600;
      }

      h1 {
        font-family: var(--font-display);
        font-size: clamp(2rem, 4vw, 2.85rem);
        margin: 0 0 0.8rem;
        color: var(--ink);
        font-weight: 700;
        line-height: 1.05;
        letter-spacing: -0.02em;
        font-variation-settings: 'opsz' 144, 'SOFT' 40;
      }

      h1 em {
        font-style: italic;
        color: var(--accent);
        font-weight: 600;
        font-variation-settings: 'opsz' 144, 'SOFT' 80;
      }

      .lead {
        font-family: var(--font-body);
        margin: 0;
        color: var(--ink-soft);
        font-size: 1.1rem;
        max-width: 60ch;
        line-height: 1.55;
      }

      .selectors {
        display: grid;
        grid-template-columns: 1fr auto 1fr;
        align-items: end;
        gap: 1rem;
        margin: 2.2rem 0 1.5rem;
        padding: 1rem 1.2rem;
        background: var(--sheet);
        border: 1px solid var(--rule);
        border-radius: 0.85rem;
      }

      @media (max-width: 720px) {
        .selectors { grid-template-columns: 1fr; }
        .vs { display: none; }
      }

      .vs {
        font-family: var(--font-display);
        font-style: italic;
        font-size: 1.3rem;
        color: var(--ink-muted);
        padding-bottom: 0.45rem;
      }

      .split {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(420px, 1fr));
        gap: 1.5rem;
      }

      .panel {
        background: var(--sheet);
        border: 1px solid var(--rule);
        border-radius: 0.95rem;
        padding: 1.6rem 1.8rem 2rem;
        min-height: 320px;
      }

      .panel-head {
        margin-bottom: 1.2rem;
        padding-bottom: 0.75rem;
        border-bottom: 1px dashed var(--rule);
      }

      .panel-tag {
        font-family: var(--font-mono);
        font-size: 0.7rem;
        text-transform: uppercase;
        letter-spacing: 0.1em;
        color: var(--accent);
        font-weight: 600;
      }

      .panel h2 {
        font-family: var(--font-display);
        font-size: 1.45rem;
        margin: 0.25rem 0 0;
        color: var(--ink);
        font-weight: 600;
        letter-spacing: -0.015em;
        font-variation-settings: 'opsz' 144, 'SOFT' 50;
      }

      .panel-body {
        transition: filter 220ms var(--ease-out), opacity 220ms var(--ease-out);
      }

      .panel-body.is-transitioning {
        filter: blur(2px);
        opacity: 0.5;
      }
    `,
  ],
})
export class ComparePage {
  readonly a = input<string | undefined>();
  readonly b = input<string | undefined>();

  private readonly content = inject(ContentService);
  private readonly router = inject(Router);
  private readonly seo = inject(SeoService);

  protected readonly leftSlug = signal(ARCHITECTURES[0].slug);
  protected readonly rightSlug = signal(ARCHITECTURES[1].slug);
  protected readonly left = signal<Side>({ slug: ARCHITECTURES[0].slug, diagram: null, description: null, loading: true });
  protected readonly right = signal<Side>({ slug: ARCHITECTURES[1].slug, diagram: null, description: null, loading: true });

  protected readonly leftArch = computed(() => findArchitecture(this.leftSlug()));
  protected readonly rightArch = computed(() => findArchitecture(this.rightSlug()));

  constructor() {
    effect(() => {
      const a = this.a();
      const b = this.b();
      if (a && findArchitecture(a)) this.leftSlug.set(a);
      if (b && findArchitecture(b) && b !== this.leftSlug()) this.rightSlug.set(b);
      this.seo.update({
        title: 'Comparar arquiteturas · ArchStudy',
        description:
          'Coloque duas arquiteturas lado a lado para comparar diagramas, descrições e estrutura de pastas.',
        type: 'website',
      });
    });

    effect(() => {
      const slug = this.leftSlug();
      this.loadSide(slug, this.left);
    });

    effect(() => {
      const slug = this.rightSlug();
      this.loadSide(slug, this.right);
    });
  }

  protected setLeft(slug: string) {
    if (slug === this.rightSlug()) return;
    this.leftSlug.set(slug);
    this.updateUrl();
  }

  protected setRight(slug: string) {
    if (slug === this.leftSlug()) return;
    this.rightSlug.set(slug);
    this.updateUrl();
  }

  private updateUrl() {
    this.router.navigate(['/compare'], {
      queryParams: { a: this.leftSlug(), b: this.rightSlug() },
      replaceUrl: true,
    });
  }

  private loadSide(slug: string, target: { set: (s: Side) => void }) {
    target.set({ slug, diagram: null, description: null, loading: true });
    Promise.all([this.content.loadDiagram(slug), this.content.loadDescription(slug)])
      .then(([diagram, description]) => {
        target.set({ slug, diagram, description, loading: false });
      })
      .catch(() => {
        target.set({ slug, diagram: null, description: null, loading: false });
      });
  }
}
