import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

export interface Architecture {
  slug: string;
  name: string;
  order: number;
  tagline: string;
  category: 'camadas' | 'features' | 'dominio';
  prefix: string;
}

export const ARCHITECTURES: readonly Architecture[] = [
  {
    slug: 'mvc',
    name: 'MVC + Services + Repositories',
    order: 1,
    tagline: 'N-tier clássico: Controller → Service → Repository → DB. Anemia controlada.',
    category: 'camadas',
    prefix: '/mvc',
  },
  {
    slug: 'feature-based',
    name: 'Feature-Based',
    order: 2,
    tagline: 'Organização por feature em vez de camada técnica. Cada feature mantém sub-camadas.',
    category: 'features',
    prefix: '/feature-based',
  },
  {
    slug: 'vertical-slice',
    name: 'Vertical Slice',
    order: 3,
    tagline: 'Cada caso de uso é UM handler. Sem camadas internas, duplicação aceitável.',
    category: 'features',
    prefix: '/vertical-slice',
  },
  {
    slug: 'onion',
    name: 'Onion Architecture',
    order: 4,
    tagline: 'Camadas concêntricas. Dependências apontam só para dentro do domínio.',
    category: 'camadas',
    prefix: '/onion',
  },
  {
    slug: 'hexagonal',
    name: 'Hexagonal (Ports & Adapters)',
    order: 5,
    tagline: 'Domínio no centro, portas (interfaces) nos lados, adaptadores fora.',
    category: 'camadas',
    prefix: '/hexagonal',
  },
  {
    slug: 'clean',
    name: 'Clean Architecture',
    order: 6,
    tagline: 'Entities, Use Cases isolados, Adapters, Frameworks. Result<T> para erros.',
    category: 'camadas',
    prefix: '/clean',
  },
  {
    slug: 'ddd',
    name: 'Domain-Driven Design (tático)',
    order: 7,
    tagline: 'Modelo de domínio rico: Aggregate Root, Value Objects, Domain Service.',
    category: 'dominio',
    prefix: '/ddd',
  },
  {
    slug: 'cqrs',
    name: 'CQRS',
    order: 8,
    tagline: 'Commands e Queries separados. Modelos de leitura e escrita distintos.',
    category: 'dominio',
    prefix: '/cqrs',
  },
] as const;

export function findArchitecture(slug: string): Architecture | undefined {
  return ARCHITECTURES.find((a) => a.slug === slug);
}

@Injectable({ providedIn: 'root' })
export class ContentService {
  private readonly http = inject(HttpClient);
  private readonly cache = new Map<string, Promise<string>>();

  loadDiagram(slug: string): Promise<string> {
    return this.load(`content/${slug}/diagram.mmd`);
  }

  loadDescription(slug: string): Promise<string> {
    return this.load(`content/${slug}/description.md`);
  }

  private load(path: string): Promise<string> {
    let pending = this.cache.get(path);
    if (!pending) {
      pending = firstValueFrom(this.http.get(path, { responseType: 'text' }));
      this.cache.set(path, pending);
    }
    return pending;
  }
}
