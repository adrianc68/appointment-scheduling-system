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
import { ClockFormatService } from '../../../cross-cutting/operation-management/clock-format-service/clock-format.service';
import { ButtonTaskComponent } from '../../ui-components/others/button-task/button-task.component';
import { LoadingState } from '../../model/loading-state.type';

@Component({
  selector: 'app-config-management',
  imports: [CommonModule, ...SHARED_STANDALONE_COMPONENTS, TranslatePipe, FormsModule, ReadableDatePipe, ButtonTaskComponent],
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
  hour12!: boolean;

  constructor(private themeService: ThemeService, private i18nService: I18nService, private timeZoneService: TimeZoneService, private clockFormatService: ClockFormatService) {
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

    this.clockFormatService.hour12$.subscribe(hour12 => {
      this.hour12 = hour12;
    });
  }

  setHourFormat(is12: boolean) {
    this.hour12 = is12;
  }




  toggleTheme() {
    this.themeService.setTheme(this.currentTheme);
  }

  setTheme(theme: 'light' | 'dark' | 'default') {
    this.themeService.setTheme(theme);
  }


  changeLanguage(lang: LanguageTypes) {
    this.i18nService.setLanguage(lang);

  }

  changeTimeZone() {
    this.timezoneState = LoadingState.LOADING;

    if (this.selectedTimeZone && this.availableTimeZones.includes(this.selectedTimeZone)) {
      this.timeZoneService.setTimeZone(this.selectedTimeZone);

      setTimeout(() => {
        this.timezoneState = LoadingState.SUCCESSFUL_TASK;
      }, 500);
    } else {
      this.timezoneState = LoadingState.UNSUCCESSFUL_TASK;
    }
  }


  setClockFormat() {
    this.clockFormatState = LoadingState.LOADING;
    this.clockFormatService.setHour12(this.hour12);

    setTimeout(() => {
      this.clockFormatState = LoadingState.SUCCESSFUL_TASK;
    }, 500);
  }

  changeCurrency() {
    this.localCurrencyState = LoadingState.LOADING;
    setTimeout(() => {
      this.localCurrencyState = LoadingState.SUCCESSFUL_TASK;
    }, 500);

  }

  localCurrencyState: LoadingState = LoadingState.NO_ACTION_PERFORMED;
  timezoneState: LoadingState = LoadingState.NO_ACTION_PERFORMED;
  clockFormatState: LoadingState = LoadingState.NO_ACTION_PERFORMED;




}
