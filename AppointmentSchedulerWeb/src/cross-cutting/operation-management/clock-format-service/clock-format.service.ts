import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';


const CLOCK_FORMAT_KEY = 'clock_format_12h';

@Injectable({
  providedIn: 'root'
})
export class ClockFormatService {
  private hour12Subject: BehaviorSubject<boolean>;


  constructor() {
    const savedValue = localStorage.getItem(CLOCK_FORMAT_KEY);
    const initial = savedValue !== null ? savedValue === 'true' : true;
    this.hour12Subject = new BehaviorSubject<boolean>(initial);
  }

  get hour12$(): Observable<boolean> {
    return this.hour12Subject.asObservable();
  }

  get currentHour12(): boolean {
    return this.hour12Subject.getValue();
  }

  setHour12(hour12: boolean) {
    this.hour12Subject.next(hour12);
    localStorage.setItem(CLOCK_FORMAT_KEY, hour12.toString());
  }
}
