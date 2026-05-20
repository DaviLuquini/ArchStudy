import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ARCHITECTURES } from '../data/architectures';

@Component({
  selector: 'app-home',
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="hero">
      <p class="hero-eyebrow">
        <span class="dot" aria-hidden="true"></span>
        Manual visual de arquiteturas
      </p>
      <h1 class="hero-title">
        Um <em>mesmo</em> domínio,
        <span class="hero-em">oito</span> arquiteturas.
      </h1>
      <p class="hero-lead">
        Conta bancária com depósito, saque, transferência e extrato — implementada em 8 estilos
        arquiteturais diferentes em .NET 10. Compare diagramas, leia trade-offs e abra o código real
        no editor para ver as diferenças onde elas importam.
      </p>
      <div class="hero-actions">
        <a class="btn btn-primary" routerLink="/arch/mvc">
          Começar pelo MVC
          <span class="btn-arrow" aria-hidden="true">→</span>
        </a>
        <a class="btn btn-secondary" routerLink="/compare">Comparar duas arquiteturas</a>
      </div>
    </section>

    <section class="catalog" aria-labelledby="catalog-heading">
      <div class="catalog-head">
        <h2 id="catalog-heading">As 8 arquiteturas</h2>
        <p class="catalog-sub">Do mais familiar (#1) ao mais denso (#8). Cada cartão abre a página dedicada.</p>
      </div>
      <ul class="catalog-grid">
        @for (arch of architectures; track arch.slug) {
          <li class="catalog-item" [style.--stagger-index]="$index">
            <a class="card" [routerLink]="['/arch', arch.slug]">
              <div class="card-top">
                <span class="card-order">{{ orderLabel(arch.order) }}</span>
                <span class="card-category" [attr.data-category]="arch.category">{{
                  categoryLabel(arch.category)
                }}</span>
              </div>
              <h3 class="card-title">{{ arch.name }}</h3>
              <p class="card-tagline">{{ arch.tagline }}</p>
              <span class="card-cta">
                Explorar
                <span class="card-arrow" aria-hidden="true">→</span>
              </span>
            </a>
          </li>
        }
      </ul>
    </section>
  `,
  styles: [
    `
      :host {
        display: block;
        padding: 3.5rem 1.5rem 5rem;
        max-width: 1100px;
        margin: 0 auto;
      }

      .hero {
        max-width: 760px;
        margin-bottom: 4rem;
      }

      .hero-eyebrow {
        display: inline-flex;
        align-items: center;
        gap: 0.5rem;
        text-transform: uppercase;
        letter-spacing: 0.16em;
        font-family: var(--font-sans);
        font-size: 0.72rem;
        color: var(--ink-muted);
        margin: 0 0 1.2rem;
        font-weight: 600;
      }

      .hero-eyebrow .dot {
        width: 6px;
        height: 6px;
        border-radius: 999px;
        background: var(--accent);
        display: inline-block;
        animation: pulse 2.4s var(--ease-in-out) infinite;
      }

      @keyframes pulse {
        0%, 100% { opacity: 1; transform: scale(1); }
        50% { opacity: 0.45; transform: scale(0.7); }
      }

      .hero-title {
        font-family: var(--font-display);
        font-size: clamp(2.4rem, 5vw, 3.8rem);
        margin: 0 0 1.5rem;
        line-height: 1.02;
        color: var(--ink);
        font-weight: 700;
        letter-spacing: -0.02em;
        font-variation-settings: 'opsz' 144, 'SOFT' 30;
      }

      .hero-title em {
        font-style: italic;
        font-variation-settings: 'opsz' 144, 'SOFT' 80;
        font-weight: 600;
      }

      .hero-em {
        color: var(--accent);
        font-style: italic;
        font-weight: 600;
        font-variation-settings: 'opsz' 144, 'SOFT' 80;
      }

      .hero-lead {
        font-family: var(--font-body);
        font-size: 1.18rem;
        color: var(--ink-soft);
        line-height: 1.62;
        font-weight: 400;
      }

      .hero-actions {
        display: flex;
        gap: 0.6rem;
        flex-wrap: wrap;
        margin-top: 2rem;
      }

      .btn {
        display: inline-flex;
        align-items: center;
        gap: 0.4rem;
        padding: 0.78rem 1.3rem;
        border-radius: 0.55rem;
        font-family: var(--font-sans);
        font-weight: 500;
        font-size: 0.95rem;
        text-decoration: none;
        border: 1px solid transparent;
        transition: transform 160ms var(--ease-out), background 200ms var(--ease-out),
          border-color 200ms var(--ease-out), color 200ms var(--ease-out);
      }

      .btn:active { transform: scale(0.97); }

      .btn-primary {
        background: var(--ink);
        color: var(--paper);
      }

      .btn-arrow {
        display: inline-block;
        transition: transform 200ms var(--ease-out);
      }

      @media (hover: hover) and (pointer: fine) {
        .btn-primary:hover { background: var(--accent); }
        .btn-primary:hover .btn-arrow { transform: translateX(3px); }
        .btn-secondary:hover { border-color: var(--ink); color: var(--ink); }
      }

      .btn-secondary {
        border-color: var(--rule);
        color: var(--ink-soft);
        background: var(--sheet);
      }

      .catalog-head {
        margin-bottom: 1.5rem;
        display: flex;
        align-items: baseline;
        justify-content: space-between;
        gap: 1rem;
        flex-wrap: wrap;
        padding-bottom: 1rem;
        border-bottom: 1px solid var(--rule);
      }

      .catalog h2 {
        font-family: var(--font-display);
        font-size: 1.4rem;
        font-style: italic;
        font-weight: 600;
        color: var(--ink);
        margin: 0;
        letter-spacing: -0.01em;
      }

      .catalog-sub {
        margin: 0;
        font-family: var(--font-sans);
        font-size: 0.85rem;
        color: var(--ink-muted);
      }

      .catalog-grid {
        list-style: none;
        margin: 0;
        padding: 0;
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
        gap: 1rem;
      }

      .catalog-item {
        opacity: 0;
        transform: translateY(8px);
        animation: card-in 420ms var(--ease-out) forwards;
        animation-delay: calc(var(--stagger-index, 0) * 55ms);
      }

      @keyframes card-in {
        to { opacity: 1; transform: translateY(0); }
      }

      .card {
        display: flex;
        flex-direction: column;
        gap: 0.55rem;
        padding: 1.4rem 1.4rem 1.25rem;
        border-radius: 0.85rem;
        border: 1px solid var(--rule);
        background: var(--sheet);
        color: inherit;
        text-decoration: none;
        height: 100%;
        position: relative;
        overflow: hidden;
        transition: transform 200ms var(--ease-out), border-color 200ms var(--ease-out),
          box-shadow 240ms var(--ease-out);
      }

      .card::before {
        content: '';
        position: absolute;
        inset: 0;
        background: linear-gradient(135deg, var(--accent-tint) 0%, transparent 45%);
        opacity: 0;
        transition: opacity 280ms var(--ease-out);
        pointer-events: none;
      }

      @media (hover: hover) and (pointer: fine) {
        .card:hover {
          border-color: var(--ink);
          transform: translateY(-3px);
          box-shadow: 0 12px 32px -16px rgba(20, 17, 12, 0.22);
        }
        .card:hover::before { opacity: 1; }
        .card:hover .card-arrow { transform: translateX(4px); }
      }

      .card:active { transform: translateY(-1px) scale(0.99); }

      .card-top {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 0.5rem;
      }

      .card-order {
        font-family: var(--font-mono);
        font-size: 0.78rem;
        color: var(--ink-muted);
        font-weight: 500;
        letter-spacing: 0.04em;
      }

      .card-category {
        font-family: var(--font-sans);
        font-size: 0.68rem;
        padding: 0.18rem 0.55rem;
        border-radius: 999px;
        background: var(--cat-camadas-bg);
        color: var(--cat-camadas-ink);
        text-transform: uppercase;
        letter-spacing: 0.08em;
        font-weight: 600;
      }

      .card-category[data-category='dominio'] {
        background: var(--cat-dominio-bg);
        color: var(--cat-dominio-ink);
      }

      .card-category[data-category='features'] {
        background: var(--cat-features-bg);
        color: var(--cat-features-ink);
      }

      .card-title {
        font-family: var(--font-display);
        font-size: 1.4rem;
        margin: 0.3rem 0 0;
        color: var(--ink);
        font-weight: 600;
        letter-spacing: -0.015em;
        line-height: 1.15;
      }

      .card-tagline {
        margin: 0;
        color: var(--ink-soft);
        font-family: var(--font-body);
        font-size: 1rem;
        line-height: 1.55;
      }

      .card-cta {
        margin-top: auto;
        padding-top: 0.6rem;
        font-family: var(--font-sans);
        font-size: 0.85rem;
        color: var(--accent);
        font-weight: 500;
        display: inline-flex;
        align-items: center;
        gap: 0.35rem;
      }

      .card-arrow {
        transition: transform 220ms var(--ease-out);
      }
    `,
  ],
})
export class HomePage {
  protected readonly architectures = ARCHITECTURES;

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
}
