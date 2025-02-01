import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoggingService {

  constructor() { }

  log(message: any): void {
    console.log(`INFO: ${message}`);
  }

  warn(message: any): void {
    console.warn(`WARN: ${message}`);
  }

  error(message: any): void {
    console.error(`ERROR: ${message}`);
  }

}
