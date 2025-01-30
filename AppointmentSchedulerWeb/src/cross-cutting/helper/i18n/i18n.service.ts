import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { LanguageTypes } from './model/languages.types';
import { HttpClientService } from '../../communication/http-client-service/http-client.service';
import { getStringEnumKeyByValue } from '../enum-utils/enum.utils';
import { LocalStorageService } from '../../security/local-storage/local-storage.service';

@Injectable({
  providedIn: 'root'
})
export class I18nService {
  private currentLanguage = new BehaviorSubject<LanguageTypes>(LanguageTypes.es_MX);
  private translations = new BehaviorSubject<{ [key: string]: string }>({});

  private isTranslationsLoaded = new BehaviorSubject<boolean>(false);


  constructor(private httpClientService: HttpClientService, private localStorageService: LocalStorageService) {
    let storedLanguage: LanguageTypes | null = this.localStorageService.getItem<LanguageTypes>("userLanguage");
    if (storedLanguage) {

      this.setLanguage(storedLanguage);
    }
  }

  setLanguage(lang: LanguageTypes): void {
    this.localStorageService.setItem('userLanguage', lang);
    this.currentLanguage.next(lang);
    this.loadTranslations(lang);
  }

  getLanguage(): LanguageTypes {
    return this.currentLanguage.getValue();
  }

  getTranslationsLoaded(): Observable<boolean> {
    return this.isTranslationsLoaded.asObservable();
  }

  getTranslations(): Observable<{ [key: string]: string }> {
    return this.translations.asObservable();
  }

  private loadTranslations(lang: LanguageTypes): void {
    const langKey = getStringEnumKeyByValue(LanguageTypes, lang);

    this.httpClientService.get<{ [key: string]: string }>(`i18n/${langKey}.json`).subscribe({
      next: (response) => {
        this.translations.next(response);
        this.isTranslationsLoaded.next(true);
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

  translate(key: string): string {
    return this.translations.getValue()[key] || key;
  }


}
