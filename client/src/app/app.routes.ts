import { provideHttpClient } from '@angular/common/http';
import { Routes } from '@angular/router';
import { ConfigurationService } from './shared/services/configuration.service';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/todo/todo.page').then((c) => c.TodoPage),
    providers: [provideHttpClient(), ConfigurationService],
  },
];
