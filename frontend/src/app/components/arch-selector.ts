import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  computed,
  effect,
  inject,
  input,
  output,
  signal,
  viewChild,
} from '@angular/core';
import { ARCHITECTURES } from '../data/architectures';

let instanceCounter = 0;

@Component({
  selector: 'app-arch-selector',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '(document:pointerdown)': 'onDocumentPointerDown($event)',
  },
  template: `
    <div class="selector">
      <span class="selector-label" [id]="labelId">{{ label() }}</span>
      <div class="selector-field">
        <button
          #trigger
          type="button"
          class="selector-trigger"
          [class.is-open]="open()"
          [attr.aria-labelledby]="labelId + ' ' + valueId"
          [attr.aria-expanded]="open()"
          [attr.aria-controls]="open() ? listboxId : null"
          aria-haspopup="listbox"
          (click)="toggle()"
          (keydown)="onTriggerKeydown($event)"
        >
          <span class="selector-value" [id]="valueId">{{ selected()?.name }}</span>
          <svg
            class="selector-chevron"
            [class.is-open]="open()"
            viewBox="0 0 12 8"
            aria-hidden="true"
            focusable="false"
          >
            <path
              d="M1 1.5 6 6.5 11 1.5"
              fill="none"
              stroke="currentColor"
              stroke-width="1.7"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
        </button>

        @if (open()) {
          <ul
            #listbox
            class="selector-list"
            role="listbox"
            tabindex="-1"
            [id]="listboxId"
            [attr.aria-label]="label()"
            [attr.aria-activedescendant]="activeOptionId()"
            (keydown)="onListboxKeydown($event)"
          >
            @for (arch of options(); track arch.slug; let i = $index) {
              <li
                class="selector-option"
                role="option"
                [id]="optionId(arch.slug)"
                [class.is-active]="i === activeIndex()"
                [attr.aria-selected]="arch.slug === value()"
                (click)="select(arch.slug)"
                (mouseenter)="activeIndex.set(i)"
              >
                <span class="option-name">{{ arch.name }}</span>
                @if (arch.slug === value()) {
                  <svg class="option-check" viewBox="0 0 14 14" aria-hidden="true" focusable="false">
                    <path
                      d="M2 7.5 5.5 11 12 3.5"
                      fill="none"
                      stroke="currentColor"
                      stroke-width="2"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                    />
                  </svg>
                }
              </li>
            }
          </ul>
        }
      </div>
    </div>
  `,
  styles: [
    `
      :host {
        display: block;
      }

      .selector {
        display: flex;
        flex-direction: column;
        gap: 0.4rem;
      }

      .selector-label {
        font-family: var(--font-sans);
        font-size: 0.72rem;
        text-transform: uppercase;
        letter-spacing: 0.1em;
        color: var(--ink-soft);
        font-weight: 600;
      }

      .selector-field {
        position: relative;
      }

      .selector-trigger {
        width: 100%;
        display: flex;
        align-items: center;
        gap: 0.75rem;
        padding: 0.7rem 0.95rem;
        border: 1px solid var(--rule);
        background: var(--sheet);
        border-radius: 0.85rem;
        font-family: var(--font-sans);
        font-size: 0.98rem;
        color: var(--ink);
        text-align: left;
        cursor: pointer;
        transition: border-color 180ms var(--ease-out), background 180ms var(--ease-out),
          box-shadow 180ms var(--ease-out), transform 140ms var(--ease-out);
      }

      .selector-trigger:hover {
        border-color: var(--ink-muted);
        background: var(--paper);
      }

      .selector-trigger:active {
        transform: scale(0.985);
      }

      .selector-trigger:focus-visible {
        outline: 2px solid var(--accent);
        outline-offset: 2px;
        border-radius: 0.85rem;
      }

      .selector-trigger.is-open {
        border-color: var(--accent);
        background: var(--sheet);
        box-shadow: 0 0 0 3px var(--accent-tint);
      }

      .selector-value {
        flex: 1;
        min-width: 0;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
      }

      .selector-chevron {
        flex-shrink: 0;
        width: 0.72rem;
        height: 0.48rem;
        color: var(--ink-muted);
        transition: transform 240ms var(--ease-out), color 180ms var(--ease-out);
      }

      .selector-chevron.is-open {
        transform: rotate(180deg);
        color: var(--accent);
      }

      .selector-list {
        position: absolute;
        z-index: 50;
        top: calc(100% + 0.4rem);
        left: 0;
        right: 0;
        margin: 0;
        padding: 0.35rem;
        list-style: none;
        background: var(--sheet);
        border: 1px solid var(--rule);
        border-radius: 0.85rem;
        box-shadow: 0 16px 34px -14px rgba(12, 35, 64, 0.18),
          0 2px 8px -4px rgba(12, 35, 64, 0.08);
        max-height: 20rem;
        overflow-y: auto;
        transform-origin: top center;
        animation: list-in 170ms var(--ease-out) both;
      }

      @keyframes list-in {
        from {
          opacity: 0;
          transform: translateY(-6px) scale(0.97);
        }
        to {
          opacity: 1;
          transform: translateY(0) scale(1);
        }
      }

      .selector-list:focus-visible {
        outline: 2px solid var(--accent);
        outline-offset: 2px;
        border-radius: 0.85rem;
      }

      .selector-option {
        display: flex;
        align-items: center;
        gap: 0.6rem;
        padding: 0.6rem 0.7rem;
        border-radius: 0.6rem;
        font-family: var(--font-sans);
        font-size: 0.95rem;
        color: var(--ink-soft);
        cursor: pointer;
        transition: background 130ms var(--ease-out), color 130ms var(--ease-out);
      }

      .selector-option + .selector-option {
        margin-top: 0.12rem;
      }

      .selector-option[aria-selected='true'] {
        color: var(--ink);
        font-weight: 600;
      }

      .selector-option.is-active {
        background: var(--accent);
        color: var(--paper);
      }

      .option-name {
        flex: 1;
      }

      .option-check {
        flex-shrink: 0;
        width: 0.9rem;
        height: 0.9rem;
        color: var(--accent);
      }

      .selector-option.is-active .option-check {
        color: var(--paper);
      }
    `,
  ],
})
export class ArchSelector {
  readonly label = input.required<string>();
  readonly value = input.required<string>();
  readonly exclude = input<string>();
  readonly valueChange = output<string>();

  private readonly hostEl = inject(ElementRef);
  private readonly triggerRef = viewChild<ElementRef<HTMLButtonElement>>('trigger');
  private readonly listboxRef = viewChild<ElementRef<HTMLUListElement>>('listbox');

  private readonly instanceId = ++instanceCounter;
  protected readonly labelId = `arch-sel-label-${this.instanceId}`;
  protected readonly valueId = `arch-sel-value-${this.instanceId}`;
  protected readonly listboxId = `arch-sel-list-${this.instanceId}`;

  protected readonly open = signal(false);
  protected readonly activeIndex = signal(0);

  protected readonly options = computed(() => {
    const skip = this.exclude();
    return ARCHITECTURES.filter((arch) => arch.slug !== skip);
  });

  protected readonly selected = computed(() =>
    ARCHITECTURES.find((arch) => arch.slug === this.value()),
  );

  protected readonly activeOptionId = computed(() => {
    const arch = this.options()[this.activeIndex()];
    return arch ? this.optionId(arch.slug) : null;
  });

  constructor() {
    effect(() => {
      if (this.open()) {
        this.listboxRef()?.nativeElement.focus();
      }
    });
  }

  protected optionId(slug: string): string {
    return `arch-sel-opt-${this.instanceId}-${slug}`;
  }

  protected toggle(): void {
    if (this.open()) {
      this.close(false);
    } else {
      this.openList();
    }
  }

  protected select(slug: string): void {
    this.valueChange.emit(slug);
    this.close(true);
  }

  protected onTriggerKeydown(event: KeyboardEvent): void {
    if (this.open()) {
      return;
    }
    if (event.key === 'ArrowDown' || event.key === 'ArrowUp') {
      event.preventDefault();
      this.openList();
    }
  }

  protected onListboxKeydown(event: KeyboardEvent): void {
    const count = this.options().length;
    switch (event.key) {
      case 'ArrowDown':
        event.preventDefault();
        this.activeIndex.update((i) => Math.min(count - 1, i + 1));
        break;
      case 'ArrowUp':
        event.preventDefault();
        this.activeIndex.update((i) => Math.max(0, i - 1));
        break;
      case 'Home':
        event.preventDefault();
        this.activeIndex.set(0);
        break;
      case 'End':
        event.preventDefault();
        this.activeIndex.set(count - 1);
        break;
      case 'Enter':
      case ' ': {
        event.preventDefault();
        const arch = this.options()[this.activeIndex()];
        if (arch) {
          this.select(arch.slug);
        }
        break;
      }
      case 'Escape':
        event.preventDefault();
        this.close(true);
        break;
      case 'Tab':
        this.close(true);
        break;
    }
  }

  protected onDocumentPointerDown(event: Event): void {
    if (!this.open()) {
      return;
    }
    const root = this.hostEl.nativeElement as HTMLElement;
    if (!root.contains(event.target as Node)) {
      this.close(false);
    }
  }

  private openList(): void {
    const current = this.options().findIndex((arch) => arch.slug === this.value());
    this.activeIndex.set(current >= 0 ? current : 0);
    this.open.set(true);
  }

  private close(focusTrigger: boolean): void {
    if (!this.open()) {
      return;
    }
    this.open.set(false);
    if (focusTrigger) {
      this.triggerRef()?.nativeElement.focus();
    }
  }
}
