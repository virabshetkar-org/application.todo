import { Component, inject } from '@angular/core';
import { ConfigurationService } from '../../shared/services/configuration.service';

@Component({
  selector: 'app-todo',
  imports: [],
  templateUrl: './todo.page.html',
  styleUrl: './todo.page.css',
})
export class TodoPage {
  readonly config = inject(ConfigurationService);

  readonly frontendConfig$ = this.config.getFrontendManifest();
  readonly backendConfig$ = this.config.getBackendConfig();
}
