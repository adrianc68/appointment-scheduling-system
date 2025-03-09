import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private readonly apiBaseUrl = environment.apiBaseUrl;


  constructor() {
  }


  getApiBaseUrl(): string {
    return this.apiBaseUrl;
  }
}

