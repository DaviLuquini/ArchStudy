import { ChangeDetectionStrategy, Component, computed, input, ViewEncapsulation } from '@angular/core';
import { marked } from 'marked';

marked.use({ gfm: true, breaks: false });

@Component({
  selector: 'app-markdown-view',
  changeDetection: ChangeDetectionStrategy.OnPush,
  encapsulation: ViewEncapsulation.None,
  template: `<div class="md-prose" [innerHTML]="html()"></div>`,
  styles: [
    `
      :host { display: block; }

      .md-prose {
        color: var(--ink-soft);
        font-family: var(--font-body);
        font-size: 1.08rem;
        line-height: 1.7;
        font-feature-settings: 'liga' 1, 'kern' 1;
      }

      .md-prose > :first-child { margin-top: 0; }

      .md-prose :where(h1, h2, h3, h4) {
        font-family: var(--font-display);
        color: var(--ink);
        line-height: 1.2;
        letter-spacing: -0.015em;
        margin: 2rem 0 0.85rem;
      }

      .md-prose :where(h2) {
        font-size: 1.4rem;
        font-style: italic;
        font-weight: 600;
        font-variation-settings: 'opsz' 144, 'SOFT' 70;
        position: relative;
        padding-left: 1rem;
      }

      .md-prose :where(h2)::before {
        content: '';
        position: absolute;
        left: 0;
        top: 0.55em;
        width: 4px;
        height: 0.7em;
        background: var(--accent);
        border-radius: 1px;
      }

      .md-prose :where(h3) {
        font-size: 1.15rem;
        font-weight: 600;
        color: var(--ink);
      }

      .md-prose :where(p) {
        margin: 0.9rem 0;
        color: var(--ink-soft);
      }

      .md-prose :where(p) strong,
      .md-prose :where(li) strong {
        color: var(--ink);
        font-weight: 600;
      }

      .md-prose :where(em) {
        font-style: italic;
        font-variation-settings: 'opsz' 18, 'SOFT' 70;
      }

      .md-prose :where(ul, ol) {
        margin: 0.8rem 0;
        padding-left: 1.4rem;
      }

      .md-prose :where(ul) { list-style: none; padding-left: 0; }

      .md-prose :where(ul) > li {
        position: relative;
        padding-left: 1.4rem;
        margin: 0.45rem 0;
      }

      .md-prose :where(ul) > li::before {
        content: '';
        position: absolute;
        left: 0.3rem;
        top: 0.72em;
        width: 6px;
        height: 1px;
        background: var(--accent);
      }

      .md-prose :where(ol) {
        counter-reset: list;
      }

      .md-prose :where(ol) > li {
        counter-increment: list;
        position: relative;
        padding-left: 1.6rem;
        margin: 0.45rem 0;
        list-style: none;
      }

      .md-prose :where(ol) > li::before {
        content: counter(list, decimal-leading-zero);
        position: absolute;
        left: 0;
        top: 0.02em;
        font-family: var(--font-mono);
        font-size: 0.78rem;
        color: var(--accent);
        font-weight: 600;
      }

      .md-prose :where(code) {
        font-family: var(--font-mono);
        background: var(--paper-deep);
        padding: 0.12rem 0.4rem;
        border-radius: 0.28rem;
        font-size: 0.86em;
        color: var(--ink);
        border: 1px solid var(--rule-soft);
      }

      .md-prose :where(pre) {
        background: var(--ink);
        color: var(--paper);
        padding: 1.1rem 1.3rem;
        border-radius: 0.55rem;
        overflow-x: auto;
        font-size: 0.88rem;
        line-height: 1.6;
        margin: 1.2rem 0;
        border: 1px solid var(--ink);
        box-shadow: 0 1px 0 var(--rule);
      }

      .md-prose :where(pre code) {
        background: transparent;
        color: inherit;
        padding: 0;
        border: none;
        font-size: inherit;
      }

      .md-prose :where(blockquote) {
        margin: 1.4rem 0;
        padding: 1rem 1.2rem;
        background: var(--accent-tint);
        border-left: 3px solid var(--accent);
        border-radius: 0 0.4rem 0.4rem 0;
        font-style: italic;
        color: var(--ink);
        font-size: 1.05rem;
        line-height: 1.55;
      }

      .md-prose :where(blockquote) p {
        margin: 0;
        color: var(--ink);
      }

      .md-prose :where(blockquote) p + p { margin-top: 0.7rem; }

      .md-prose :where(hr) {
        border: none;
        text-align: center;
        margin: 2rem 0;
        line-height: 1;
        color: var(--ink-muted);
      }

      .md-prose :where(hr)::after {
        content: '✦ ✦ ✦';
        font-size: 0.75rem;
        letter-spacing: 0.5em;
        color: var(--rule);
      }

      .md-prose :where(table) {
        width: 100%;
        border-collapse: collapse;
        margin: 1.4rem 0;
        font-family: var(--font-sans);
        font-size: 0.92rem;
      }

      .md-prose :where(thead) {
        background: var(--paper-deep);
      }

      .md-prose :where(th) {
        text-align: left;
        padding: 0.6rem 0.85rem;
        font-weight: 600;
        font-family: var(--font-sans);
        font-size: 0.78rem;
        text-transform: uppercase;
        letter-spacing: 0.06em;
        color: var(--ink);
        border-bottom: 2px solid var(--rule);
      }

      .md-prose :where(td) {
        padding: 0.65rem 0.85rem;
        vertical-align: top;
        border-bottom: 1px solid var(--rule-soft);
        color: var(--ink-soft);
        line-height: 1.55;
      }

      .md-prose :where(tbody tr):last-child td {
        border-bottom: none;
      }

      .md-prose :where(a) {
        color: var(--accent);
        text-decoration: underline;
        text-decoration-thickness: 1px;
        text-underline-offset: 3px;
        transition: text-decoration-thickness 160ms var(--ease-out);
      }

      .md-prose :where(a:hover) {
        text-decoration-thickness: 2px;
      }
    `,
  ],
})
export class MarkdownView {
  readonly source = input.required<string>();
  protected readonly html = computed(() => marked.parse(this.source() ?? '', { async: false }) as string);
}
