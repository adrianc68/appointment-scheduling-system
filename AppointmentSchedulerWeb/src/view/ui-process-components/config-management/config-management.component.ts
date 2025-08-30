import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { ThemeService } from '../../../cross-cutting/operation-management/configService/theme.service';
import { LanguageTypes } from '../../../cross-cutting/helper/i18n/model/languages.types';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { TranslatePipe } from '../../../cross-cutting/helper/i18n/translate.pipe';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { TimeZoneService } from '../../../cross-cutting/operation-management/timeZoneService/time-zone.service';
import { FormsModule } from '@angular/forms';
import { ReadableDatePipe } from '../../../cross-cutting/helper/date-utils/readable-date.pipe';

@Component({
  selector: 'app-config-management',
  imports: [CommonModule, ...SHARED_STANDALONE_COMPONENTS, TranslatePipe, FormsModule, ReadableDatePipe],
  standalone: true,
  templateUrl: './config-management.component.html',
  styleUrl: './config-management.component.scss'
})
export class ConfigManagementComponent {
  TranslationCodes = TranslationCodes;
  currentTheme!: 'light' | 'dark' | 'default';
  currentLanguage!: LanguageTypes;
  currentTimeZone!: string;
  LanguagesTypes = LanguageTypes;
  availableTimeZones: string[] = [
    'America/Mexico_City',
    'America/New_York',
    'Europe/Berlin',
    'Asia/Tokyo'
  ];
  selectedTimeZone: string = '';
  now: Date = new Date();


  constructor(private themeService: ThemeService, private i18nService: I18nService, private timeZoneService: TimeZoneService) {
    this.themeService.currentTheme$.subscribe(theme => {
      this.currentTheme = theme;
    });

    this.i18nService.getLanguageAsObservable().subscribe((language: LanguageTypes) => {
      this.currentLanguage = language;
    });

    this.timeZoneService.currentTimeZone$.subscribe(tz => {
      this.currentTimeZone = tz;
      this.selectedTimeZone = tz;

    });
  }

  toggleTheme() {
    this.themeService.setTheme(this.currentTheme);
  }

  setTheme(theme: 'light' | 'dark' | 'default') {
    this.themeService.setTheme(theme);
  }

  changeLanguageToEnglish(): void {
    this.i18nService.setLanguage(LanguageTypes.en_US);
  }

  changeLanguageToSpanish(): void {
    this.i18nService.setLanguage(LanguageTypes.es_MX);
  }

  changeLanguage(lang: LanguageTypes) {
    this.i18nService.setLanguage(lang);
  }

  changeTimeZone() {
    if (this.selectedTimeZone && this.availableTimeZones.includes(this.selectedTimeZone)) {
      this.timeZoneService.setTimeZone(this.selectedTimeZone);
    } else {
      alert('Zona horaria no válida');
    }
  }


}
