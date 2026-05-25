import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  input,
  signal,
} from '@angular/core';
import { ContentService, FileEntry, FilesPayload } from '../data/architectures';
import { FileTree, FileTreeNode, FileTreeDirNode } from './file-tree';
import { FileViewer } from './file-viewer';

function buildTree(files: readonly FileEntry[]): FileTreeNode[] {
  const root: FileTreeNode[] = [];

  for (const file of files) {
    const parts = file.path.split('/');
    let level: FileTreeNode[] = root;
    let acc = '';

    for (let i = 0; i < parts.length; i++) {
      const part = parts[i];
      acc = acc ? `${acc}/${part}` : part;
      const isLast = i === parts.length - 1;

      if (isLast) {
        level.push({
          type: 'file',
          name: part,
          path: file.path,
          annotation: file.annotation,
        });
      } else {
        let dir = level.find(
          (n): n is FileTreeDirNode => n.type === 'dir' && n.name === part,
        );
        if (!dir) {
          dir = { type: 'dir', name: part, path: acc, children: [] };
          level.push(dir);
        }
        level = dir.children;
      }
    }
  }

  const sortLevel = (nodes: FileTreeNode[]): void => {
    nodes.sort((a, b) => {
      if (a.type !== b.type) return a.type === 'dir' ? -1 : 1;
      return a.name.localeCompare(b.name);
    });
    for (const n of nodes) {
      if (n.type === 'dir') sortLevel(n.children);
    }
  };
  sortLevel(root);
  return root;
}

@Component({
  selector: 'app-structure-section',
  imports: [FileTree, FileViewer],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (payload(); as p) {
      <div class="structure-root">
        <code class="root-label">{{ p.root }}/</code>
        <span class="root-meta">{{ p.files.length }} arquivos</span>
      </div>
      <app-file-tree
        [nodes]="tree()"
        [selectedPath]="selectedPath()"
        [collapsedDirs]="collapsedDirs()"
        (fileSelect)="selectFile($event)"
        (dirToggle)="toggleDir($event)"
      />
      @if (selectedFile(); as sf) {
        <app-file-viewer
          [path]="sf.path"
          [content]="sf.content"
          (close)="selectFile(null)"
        />
      }
    } @else if (loading()) {
      <p class="structure-loading">Carregando arquivos…</p>
    } @else if (error()) {
      <p class="structure-error" role="alert">Falha ao carregar arquivos.</p>
    }
  `,
  styles: [
    `
      :host { display: block; }

      .structure-root {
        display: flex;
        align-items: baseline;
        gap: 0.6rem;
        margin-bottom: 0.85rem;
        padding-bottom: 0.6rem;
        border-bottom: 1px dashed var(--rule);
      }

      .root-label {
        font-family: var(--font-mono);
        font-size: 0.85rem;
        font-weight: 600;
        color: var(--ink);
        background: transparent;
        padding: 0;
        border: none;
      }

      .root-meta {
        font-family: var(--font-sans);
        font-size: 0.74rem;
        text-transform: uppercase;
        letter-spacing: 0.08em;
        color: var(--ink-muted);
      }

      .structure-loading {
        color: var(--ink-muted);
        font-style: italic;
      }

      .structure-error { color: var(--accent); }
    `,
  ],
})
export class StructureSection {
  readonly slug = input.required<string>();

  private readonly content = inject(ContentService);

  protected readonly payload = signal<FilesPayload | null>(null);
  protected readonly loading = signal(true);
  protected readonly error = signal(false);
  protected readonly selectedPath = signal<string | null>(null);
  protected readonly collapsedDirs = signal<ReadonlySet<string>>(new Set<string>());

  protected readonly tree = computed(() => buildTree(this.payload()?.files ?? []));

  protected readonly selectedFile = computed(() => {
    const path = this.selectedPath();
    if (!path) return null;
    return this.payload()?.files.find((f) => f.path === path) ?? null;
  });

  constructor() {
    effect(() => {
      const slug = this.slug();
      this.loading.set(true);
      this.payload.set(null);
      this.selectedPath.set(null);
      this.error.set(false);
      this.content
        .loadFiles(slug)
        .then((p) => {
          this.payload.set(p);
          this.loading.set(false);
        })
        .catch(() => {
          this.error.set(true);
          this.loading.set(false);
        });
    });
  }

  protected selectFile(path: string | null): void {
    this.selectedPath.set(path);
  }

  protected toggleDir(path: string): void {
    this.collapsedDirs.update((s) => {
      const next = new Set(s);
      if (next.has(path)) next.delete(path);
      else next.add(path);
      return next;
    });
  }
}
