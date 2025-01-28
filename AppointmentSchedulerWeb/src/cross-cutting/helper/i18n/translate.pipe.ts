import { Pipe, PipeTransform } from '@angular/core';
import { I18nService } from './i18n.service';
import { TranslationCodes } from './model/translation-codes.types';

@Pipe({
  name: 'translate',
  standalone: true,
  pure: false
})

export class TranslatePipe implements PipeTransform {
  constructor(private i18nService: I18nService) { }

  transform(key: TranslationCodes): string {
    console.log(key);
    return this.i18nService.translate(key);
  }
}
