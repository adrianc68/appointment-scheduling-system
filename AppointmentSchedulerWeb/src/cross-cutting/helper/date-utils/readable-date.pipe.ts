import { Pipe, PipeTransform } from '@angular/core';
import { I18nService } from '../i18n/i18n.service';
import { LanguageTypes } from '../i18n/model/languages.types';

@Pipe({
  name: 'readableDate',
  standalone: true,
  pure: false
})
export class ReadableDatePipe implements PipeTransform {


  private currentLanguage = LanguageTypes.es_MX;


  constructor(private i18nService: I18nService) {
    this.i18nService.getLanguageAsObservable().subscribe(language => {
      this.currentLanguage = language;
    });
  }

  transform(isoString: string): string {
    const date = new Date(isoString);

    const formattedDate = date.toLocaleDateString(this.getCurrentLanguage(), {
      day: '2-digit',
      month: 'long',
      year: 'numeric',
    });

    const formattedTime = date.toLocaleTimeString(this.getCurrentLanguage(), {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true,
    });

    return `${formattedDate}, ${formattedTime}`;
  }


  private getCurrentLanguage(): string {
    switch (this.currentLanguage) {
      case LanguageTypes.es_MX:
        return "es-MX";
      case LanguageTypes.en_US:
        return "en-US";
      default:
        return "es-MX"

    }
  }
}

