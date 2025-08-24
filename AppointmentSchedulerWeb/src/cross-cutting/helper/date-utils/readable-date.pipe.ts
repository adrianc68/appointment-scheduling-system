import { Pipe, PipeTransform } from '@angular/core';
import { I18nService } from '../i18n/i18n.service';
import { LanguageTypes } from '../i18n/model/languages.types';
import { TimeZoneService } from '../../operation-management/timeZoneService/time-zone.service';

@Pipe({
  name: 'readableDate',
  standalone: true,
  pure: false
})
export class ReadableDatePipe implements PipeTransform {
  private currentLanguage = LanguageTypes.es_MX;
  private currentTimeZone: string = 'UTC';

  constructor(private i18nService: I18nService, private timezoneService: TimeZoneService) {
    this.i18nService.getLanguageAsObservable().subscribe(language => {
      this.currentLanguage = language;
    });

    this.timezoneService.currentTimeZone$.subscribe(tz => {
      this.currentTimeZone = tz;
    });
  }

  transform(isoString: string | Date, timeZone?: string): string {
    const date = new Date(isoString);

    const formattedDate = date.toLocaleDateString(this.getCurrentLanguage(), {
      day: '2-digit',
      month: 'long',
      year: 'numeric',
      timeZone: timeZone || this.currentTimeZone || 'UTC',
    });

    const formattedTime = date.toLocaleTimeString(this.getCurrentLanguage(), {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true,
      timeZone: timeZone || 'UTC',
    });

    return `${formattedDate}, ${formattedTime}`;
  }

  private getCurrentLanguage(): string {
    switch (this.currentLanguage) {
      case LanguageTypes.es_MX: return 'es-MX';
      case LanguageTypes.en_US: return 'en-US';
      default: return 'es-MX';
    }
  }
}
