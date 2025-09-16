import { Pipe, PipeTransform } from '@angular/core';
import { I18nService } from '../i18n/i18n.service';
import { LanguageTypes } from '../i18n/model/languages.types';
import { TimeZoneService } from '../../operation-management/timeZoneService/time-zone.service';
import { ClockFormatService } from '../../operation-management/clock-format-service/clock-format.service';
import { isSubscription } from 'rxjs/internal/Subscription';



@Pipe({
  name: 'readableTime'
})
export class ReadableTimePipe implements PipeTransform {
  private currentLanguage = LanguageTypes.es_MX;
  private currentTimeZone: string = 'UTC';

  private hour12: boolean = true;

  constructor(private i18nService: I18nService, private timezoneService: TimeZoneService, private clockFormatService: ClockFormatService) {
    this.i18nService.getLanguageAsObservable().subscribe(language => {
      this.currentLanguage = language;
    });

    this.timezoneService.currentTimeZone$.subscribe(tz => {
      this.currentTimeZone = tz;
    });

    this.clockFormatService.hour12$.subscribe(value => {
      this.hour12 = value;
    });
  }

  transform(isoString: string | Date | null | undefined, timeZone?: string): string {
    if (!isoString) return '';


    let date: Date;

    if (typeof isoString === 'string' && /^\d{1,2}:\d{2}(:\d{2})?$/.test(isoString)) {
      const parts = isoString.split(':').map(Number);
      const [hours, minutes, seconds = 0] = parts;
      const today = new Date();
      date = new Date(today.getFullYear(), today.getMonth(), today.getDate(), hours, minutes, seconds);
    } else {
      date = new Date(isoString);
      if (isNaN(date.getTime())) return '';
    }

    const formattedTime = date.toLocaleTimeString(this.getCurrentLanguage(), {
      hour: '2-digit',
      minute: '2-digit',
      hour12: this.hour12,
      timeZone: timeZone || this.currentTimeZone || 'UTC',
    });

    return formattedTime;
  }

  private getCurrentLanguage(): string {
    switch (this.currentLanguage) {
      case LanguageTypes.es_MX: return 'es-MX';
      case LanguageTypes.en_US: return 'en-US';
      default: return 'es-MX';
    }
  }

}
