import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';

export interface FileTreeFileNode {
  type: 'file';
  name: string;
  path: string;
  annotation: string | null;
}

export interface FileTreeDirNode {
  type: 'dir';
  name: string;
  path: string;
  children: FileTreeNode[];
}

export type FileTreeNode = FileTreeFileNode | FileTreeDirNode;

@Component({
  selector: 'app-file-tree',
  imports: [NgTemplateOutlet],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <ng-template #treeTpl let-nodes="nodes" let-level="level">
      <ul class="tree" [class.is-root]="level === 0">
        @for (node of nodes; track node.path) {
          @if (node.type === 'dir') {
            <li class="tree-dir">
              <button
                type="button"
                class="tree-dir-toggle"
                [attr.aria-expanded]="!isCollapsed(node.path)"
                (click)="dirToggle.emit(node.path)"
              >
                <span class="caret" aria-hidden="true">{{ isCollapsed(node.path) ? '▸' : '▾' }}</span>
                <span class="dir-name">{{ node.name }}/</span>
              </button>
              @if (!isCollapsed(node.path)) {
                <ng-container
                  *ngTemplateOutlet="treeTpl; context: { nodes: node.children, level: level + 1 }"
                ></ng-container>
              }
            </li>
          } @else {
            <li class="tree-file" [class.is-selected]="node.path === selectedPath()">
              <button
                type="button"
                class="tree-file-btn"
                (click)="fileSelect.emit(node.path)"
              >
                <span class="file-name">{{ node.name }}</span>
                @if (node.annotation) {
                  <span class="file-annot">· {{ node.annotation }}</span>
                }
              </button>
            </li>
          }
        }
      </ul>
    </ng-template>
    <ng-container *ngTemplateOutlet="treeTpl; context: { nodes: nodes(), level: 0 }"></ng-container>
  `,
  styles: [
    `
      :host { display: block; }

      .tree {
        list-style: none;
        margin: 0;
        padding: 0 0 0 1.1rem;
        border-left: 1px solid var(--rule-soft);
      }

      .tree.is-root {
        padding-left: 0;
        border-left: none;
      }

      .tree-dir-toggle,
      .tree-file-btn {
        all: unset;
        cursor: pointer;
        display: flex;
        align-items: baseline;
        gap: 0.5rem;
        width: 100%;
        padding: 0.22rem 0.5rem;
        border-radius: 0.4rem;
        font-family: var(--font-mono);
        font-size: 0.86rem;
        color: var(--ink);
        line-height: 1.45;
        transition: background 130ms var(--ease-out), color 130ms var(--ease-out);
      }

      .tree-dir-toggle:focus-visible,
      .tree-file-btn:focus-visible {
        outline: 2px solid var(--accent);
        outline-offset: 2px;
      }

      @media (hover: hover) and (pointer: fine) {
        .tree-dir-toggle:hover,
        .tree-file-btn:hover {
          background: var(--paper-deep);
        }
      }

      .caret {
        display: inline-block;
        width: 0.9em;
        color: var(--ink-muted);
        font-size: 0.85em;
        transform: translateY(-1px);
      }

      .dir-name {
        font-weight: 600;
        color: var(--ink);
      }

      .file-name {
        color: var(--ink);
      }

      .file-annot {
        font-family: var(--font-sans);
        font-size: 0.82rem;
        color: var(--ink-muted);
        font-weight: 400;
      }

      .tree-file.is-selected .tree-file-btn {
        background: var(--accent-tint);
        color: var(--accent);
      }

      .tree-file.is-selected .file-name {
        color: var(--accent);
        font-weight: 600;
      }

      .tree-file.is-selected .file-annot {
        color: var(--accent);
        opacity: 0.75;
      }
    `,
  ],
})
export class FileTree {
  readonly nodes = input.required<FileTreeNode[]>();
  readonly selectedPath = input<string | null>(null);
  readonly collapsedDirs = input<ReadonlySet<string>>(new Set<string>());
  readonly fileSelect = output<string>();
  readonly dirToggle = output<string>();

  protected isCollapsed(path: string): boolean {
    return this.collapsedDirs().has(path);
  }
}
