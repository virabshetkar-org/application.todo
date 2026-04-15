import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ConfigurationService {
  readonly #http = inject(HttpClient);

  getFrontendManifest() {
    return this.#http.get('/config/frontend/federation.manifest.json');
  }

  getBackendConfig() {
    return this.#http.get('/api/todo/configuration');
  }
}
