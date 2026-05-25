import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  ViewEncapsulation,
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
      background: '#ffffff',
      primaryColor: '#ffffff',
      primaryTextColor: '#0c2340',
      primaryBorderColor: '#1a3a6e',
      lineColor: '#4a5568',
      secondaryColor: '#e8edf5',
      tertiaryColor: '#e8edf5',
      mainBkg: '#ffffff',
      nodeTextColor: '#0c2340',
      clusterBkg: '#e8edf5',
      clusterBorder: '#7a8aa3',
      titleColor: '#0c2340',
      edgeLabelBackground: '#ffffff',
      fontFamily: 'Newsreader, ui-serif, Georgia, serif',
      fontSize: '18px',
    },
    flowchart: {
      curve: 'basis',
      padding: 16,
      nodeSpacing: 55,
      rankSpacing: 50,
      useMaxWidth: false,
    },
  });
  initialized = true;
}

@Component({
  selector: 'app-mermaid-view',
  changeDetection: ChangeDetectionStrategy.OnPush,
  encapsulation: ViewEncapsulation.None,
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
        padding: 1.25rem 1rem;
        background: var(--paper-deep);
        border: 1px solid var(--rule);
        border-radius: 0.85rem;
        box-shadow: 0 8px 24px -12px rgba(12, 35, 64, 0.12);
      }
      .mermaid-render {
        display: flex;
        justify-content: center;
        align-items: center;
        min-height: 180px;
        overflow-x: auto;
      }
      .mermaid-render :where(svg) {
        width: 100%;
        max-height: 84vh;
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
