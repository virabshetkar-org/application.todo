import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/todo/todo.page').then((c) => c.TodoPage),
  },
];
