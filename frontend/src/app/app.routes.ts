import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./pages/home').then((m) => m.HomePage),
  },
  {
    path: 'arch/:slug',
    loadComponent: () => import('./pages/architecture').then((m) => m.ArchitecturePage),
  },
  {
    path: 'compare',
    loadComponent: () => import('./pages/compare').then((m) => m.ComparePage),
  },
  { path: '**', redirectTo: '' },
];
