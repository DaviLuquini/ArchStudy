import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  effect,
  inject,
  input,
  signal,
  viewChild,
} from '@angular/core';
import mermaid from 'mermaid';

let initialized = false;
let counter = 0;

function ensureInitialized() {
  if (initialized) return;
  mermaid.initialize({
    startOnLoad: false,
    theme: 'base',
    securityLevel: 'strict',
    fontFamily: 'Newsreader, ui-serif, Georgia, serif',
    themeVariables: {
      background: '#fffdf8',
      primaryColor: '#fbf0e6',
      primaryTextColor: '#14110c',
      primaryBorderColor: '#a14014',
      lineColor: '#756f5d',
      secondaryColor: '#efe7d6',
      tertiaryColor: '#f7f3eb',
      mainBkg: '#fffdf8',
      nodeTextColor: '#14110c',
      clusterBkg: '#f7f3eb',
      clusterBorder: '#e3d9c1',
      titleColor: '#14110c',
      edgeLabelBackground: '#fffdf8',
      fontFamily: 'Newsreader, ui-serif, Georgia, serif',
      fontSize: '14px',
    },
    flowchart: {
      curve: 'basis',
      padding: 14,
      nodeSpacing: 38,
      rankSpacing: 50,
      useMaxWidth: true,
    },
  });
  initialized = true;
}

@Component({
  selector: 'app-mermaid-view',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <figure class="mermaid-figure">
      <div class="mermaid-render" #host></div>
      @if (error()) {
        <pre class="mermaid-error" role="alert">{{ error() }}</pre>
      }
    </figure>
  `,
  styles: [
    `
      :host {
        display: block;
      }
      .mermaid-figure {
        margin: 0;
      }
      .mermaid-render {
        display: flex;
        justify-content: center;
        align-items: center;
        min-height: 320px;
        overflow-x: auto;
      }
      .mermaid-render :where(svg) {
        max-width: 100%;
        height: auto;
      }
      .mermaid-error {
        margin-top: 1rem;
        padding: 0.75rem 1rem;
        background: var(--accent-tint);
        color: var(--accent);
        border: 1px solid var(--accent-soft);
        border-radius: 0.5rem;
        font-family: var(--font-mono);
        font-size: 0.82rem;
        white-space: pre-wrap;
      }
    `,
  ],
})
export class MermaidView {
  readonly source = input.required<string>();
  readonly label = input<string>('Diagrama da arquitetura');
  protected readonly error = signal<string | null>(null);
  private readonly host = viewChild.required<ElementRef<HTMLDivElement>>('host');

  constructor() {
    effect(() => {
      const src = this.source();
      const node = this.host().nativeElement;
      if (!src) {
        node.innerHTML = '';
        return;
      }
      this.render(src, node);
    });
  }

  private async render(src: string, node: HTMLElement) {
    ensureInitialized();
    const id = `mmd-${++counter}`;
    try {
      const { svg } = await mermaid.render(id, src.trim());
      node.innerHTML = svg;
      node.setAttribute('role', 'img');
      node.setAttribute('aria-label', this.label());
      this.error.set(null);
    } catch (err) {
      const message = err instanceof Error ? err.message : String(err);
      this.error.set(message);
      node.innerHTML = '';
    }
  }
}
