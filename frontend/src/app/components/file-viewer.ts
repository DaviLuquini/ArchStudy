import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import Prism from 'prismjs';
import 'prismjs/components/prism-csharp';

@Component({
  selector: 'app-file-viewer',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <header class="viewer-header">
      <code class="viewer-path">{{ path() }}</code>
      <button
        type="button"
        class="viewer-close"
        (click)="close.emit()"
        aria-label="Fechar arquivo"
      >×</button>
    </header>
    <pre class="viewer-pre"><code class="language-csharp" [innerHTML]="highlighted()"></code></pre>
  `,
  styles: [
    `
      :host {
        display: block;
        margin-top: 1rem;
        border: 1px solid var(--rule);
        border-radius: 0.6rem;
        background: var(--sheet);
        overflow: hidden;
      }

      .viewer-header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 0.75rem;
        padding: 0.55rem 0.85rem;
        background: var(--paper-deep);
        border-bottom: 1px solid var(--rule);
      }

      .viewer-path {
        font-family: var(--font-mono);
        font-size: 0.82rem;
        color: var(--ink-soft);
        background: transparent;
        padding: 0;
        border: none;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
      }

      .viewer-close {
        all: unset;
        cursor: pointer;
        font-size: 1.25rem;
        line-height: 1;
        padding: 0.1rem 0.45rem;
        border-radius: 0.35rem;
        color: var(--ink-muted);
        transition: background 130ms var(--ease-out), color 130ms var(--ease-out);
      }

      .viewer-close:hover {
        background: var(--rule-soft);
        color: var(--ink);
      }

      .viewer-close:focus-visible {
        outline: 2px solid var(--accent);
        outline-offset: 2px;
      }

      .viewer-pre {
        margin: 0;
        padding: 1rem 1.15rem;
        background: var(--sheet);
        color: var(--ink);
        font-family: var(--font-mono);
        font-size: 0.84rem;
        line-height: 1.55;
        overflow-x: auto;
        max-height: 60vh;
      }

      .viewer-pre code {
        background: transparent;
        border: none;
        padding: 0;
        color: inherit;
        font-family: inherit;
        font-size: inherit;
      }

      /* Prism C# token palette tuned to the site's light theme */
      :host ::ng-deep .token.comment,
      :host ::ng-deep .token.prolog,
      :host ::ng-deep .token.doctype,
      :host ::ng-deep .token.cdata {
        color: var(--ink-muted);
        font-style: italic;
      }
      :host ::ng-deep .token.punctuation { color: var(--ink-soft); }
      :host ::ng-deep .token.namespace { opacity: 0.7; }
      :host ::ng-deep .token.keyword,
      :host ::ng-deep .token.boolean {
        color: var(--accent);
        font-weight: 600;
      }
      :host ::ng-deep .token.string,
      :host ::ng-deep .token.char {
        color: var(--cat-features-ink);
      }
      :host ::ng-deep .token.number {
        color: var(--cat-dominio-ink);
      }
      :host ::ng-deep .token.class-name,
      :host ::ng-deep .token.builtin,
      :host ::ng-deep .token.symbol {
        color: var(--cat-dominio-ink);
      }
      :host ::ng-deep .token.function {
        color: var(--accent);
      }
      :host ::ng-deep .token.operator,
      :host ::ng-deep .token.entity {
        color: var(--ink-soft);
      }
      :host ::ng-deep .token.attr-name,
      :host ::ng-deep .token.attr-value {
        color: var(--cat-camadas-ink);
      }
    `,
  ],
})
export class FileViewer {
  readonly path = input.required<string>();
  readonly content = input.required<string>();
  readonly close = output<void>();

  protected readonly highlighted = computed(() =>
    Prism.highlight(this.content(), Prism.languages['csharp'], 'csharp'),
  );
}
