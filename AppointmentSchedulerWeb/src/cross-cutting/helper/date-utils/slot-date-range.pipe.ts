import { Pipe, PipeTransform } from '@angular/core';
import { ClockFormatService } from '../../operation-management/clock-format-service/clock-format.service';
import { I18nService } from '../i18n/i18n.service';
import { TimeZoneService } from '../../operation-management/timeZoneService/time-zone.service';
import { LanguageTypes } from '../i18n/model/languages.types';

@Pipe({
  name: 'slotDateRange'
})
export class SlotDateRangePipe implements PipeTransform {

  private currentLanguage: string = 'es-MX';
  private currentTimeZone: string = 'UTC';
  private hour12: boolean = true;

  constructor(
    private i18nService: I18nService,
    private timezoneService: TimeZoneService,
    private clockFormatService: ClockFormatService
  ) {
    this.i18nService.getLanguageAsObservable().subscribe(language => {
      this.currentLanguage = this.mapLanguage(language);
    });

    this.timezoneService.currentTimeZone$.subscribe(tz => {
      this.currentTimeZone = tz;
    });

    this.clockFormatService.hour12$.subscribe(value => {
      this.hour12 = value;
    });
  }

  transform(start: string | Date, end: string | Date): string {
    if (!start || !end) return '';

    const startDate = start instanceof Date ? start : new Date(start);
    const endDate = end instanceof Date ? end : new Date(end);

    const startDateStr = startDate.toLocaleDateString(this.currentLanguage, {
      day: '2-digit',
      month: 'long',
      year: 'numeric',
      timeZone: this.currentTimeZone
    });

    const endDateStr = endDate.toLocaleDateString(this.currentLanguage, {
      day: '2-digit',
      month: 'long',
      year: 'numeric',
      timeZone: this.currentTimeZone
    });

    const startTimeStr = startDate.toLocaleTimeString(this.currentLanguage, {
      hour: '2-digit',
      minute: '2-digit',
      hour12: this.hour12,
      timeZone: this.currentTimeZone
    });

    const endTimeStr = endDate.toLocaleTimeString(this.currentLanguage, {
      hour: '2-digit',
      minute: '2-digit',
      hour12: this.hour12,
      timeZone: this.currentTimeZone
    });

    const datePart = startDateStr === endDateStr ? startDateStr : `${startDateStr} - ${endDateStr}`;
    return `${datePart} | ${startTimeStr} - ${endTimeStr}`;
  }

  private mapLanguage(language: LanguageTypes): string {
    switch (language) {
      case LanguageTypes.es_MX: return 'es-MX';
      case LanguageTypes.en_US: return 'en-US';
      default: return 'es-MX';
    }
  }
}
