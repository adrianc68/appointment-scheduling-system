import { Pipe, PipeTransform } from '@angular/core';
import { I18nService } from '../i18n/i18n.service';
import { LanguageTypes } from '../i18n/model/languages.types';

@Pipe({
  name: 'readableDate',
  standalone: true,
  pure: false
})
export class ReadableDatePipe implements PipeTransform {
  private currentLanguage: string = 'es-MX';

  constructor(
    private i18nService: I18nService,
  ) {
    this.i18nService.getLanguageAsObservable().subscribe(language => {
      this.currentLanguage = this.mapLanguageToString(language);
    });


  }

  transform(value: string | Date | null | undefined): string {
    if (!value) return '';

    let year: number, month: number, day: number;

    if (typeof value === 'string') {
      [year, month, day] = value.split('-').map(Number);
    } else if (value instanceof Date) {
      year = value.getFullYear();
      month = value.getMonth() + 1;
      day = value.getDate();
    } else {
      return '';
    }

    const dateForFormat = new Date(Date.UTC(year, month - 1, day, 12));

    const options: Intl.DateTimeFormatOptions = {
      year: 'numeric',
      month: 'long',
      day: '2-digit'
    };

    return new Intl.DateTimeFormat(this.currentLanguage, options).format(dateForFormat);
  }

  private mapLanguageToString(language: LanguageTypes): string {
    switch (language) {
      case LanguageTypes.es_MX: return 'es-MX';
      case LanguageTypes.en_US: return 'en-US';
      default: return 'es-MX';
    }
  }
}
