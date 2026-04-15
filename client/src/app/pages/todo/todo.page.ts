import { Component, inject } from '@angular/core';
import { ConfigurationService } from '../../shared/services/configuration.service';
import { AsyncPipe, JsonPipe } from '@angular/common';

@Component({
  selector: 'app-todo',
  imports: [AsyncPipe, JsonPipe],
  templateUrl: './todo.page.html',
  styleUrl: './todo.page.css',
})
export class TodoPage {
  readonly config = inject(ConfigurationService);

  readonly frontendConfig$ = this.config.getFrontendManifest();
  readonly backendConfig$ = this.config.getBackendConfig();
}
