import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class TimeZoneService {
  private currentTimeZoneSubject = new BehaviorSubject<string>(this.getInitialTimeZone());
  currentTimeZone$ = this.currentTimeZoneSubject.asObservable();

  constructor() {
    // Inicialmente aplicar la zona horaria guardada
    this.applyTimeZone(this.currentTimeZoneSubject.value);
  }

  setTimeZone(timeZone: string) {
    this.currentTimeZoneSubject.next(timeZone);
    this.applyTimeZone(timeZone);
    localStorage.setItem('system-timezone', timeZone);
  }

  private getInitialTimeZone(): string {
    const savedTz = localStorage.getItem('system-timezone');
    if (savedTz) return savedTz;

    try {
      const tz = Intl.DateTimeFormat().resolvedOptions().timeZone;
      return tz || 'UTC';
    } catch {
      return 'UTC';
    }
  }

  private applyTimeZone(timeZone: string) {
    document.documentElement.setAttribute('data-timezone', timeZone);
  }
}

