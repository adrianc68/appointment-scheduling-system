import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private currentThemeSubject = new BehaviorSubject<'light' | 'dark' | 'default'>(this.getInitialTheme());
  currentTheme$ = this.currentThemeSubject.asObservable();

  constructor() {
    this.applyTheme(this.currentThemeSubject.value);
    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    mediaQuery.addEventListener('change', this.onSystemThemeChange.bind(this));

    if (this.currentThemeSubject.value === 'default') {
      this.applyThemeBasedOnSystem();
    }
  }

  setTheme(theme: 'light' | 'dark' | 'default') {
    this.currentThemeSubject.next(theme);
    this.applyTheme(theme);
  }


  setCurrentTheme() {
    this.applyTheme(this.currentThemeSubject.value);
  }

  private getInitialTheme(): 'light' | 'dark' | 'default' {
    const savedTheme = localStorage.getItem('theme') as 'light' | 'dark' | 'default' | null;
    return savedTheme ?? 'default';
  }

  private applyTheme(theme: 'light' | 'dark' | 'default') {
    if (theme === 'default') {
      document.documentElement.setAttribute('data-theme', (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'));
    } else {
      document.documentElement.setAttribute('data-theme', theme);
    }
    localStorage.setItem('theme', theme);
  }

  private applyThemeBasedOnSystem() {
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    const theme = prefersDark ? 'dark' : 'light';
    document.documentElement.setAttribute('data-theme', theme);
  }

  private onSystemThemeChange(event: MediaQueryListEvent) {
    if (this.currentThemeSubject.value === 'default') {
      const theme = event.matches ? 'dark' : 'light';
      document.documentElement.setAttribute('data-theme', theme);
    }
  }
}

