import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { ThemeService } from '../../../cross-cutting/operation-management/configService/theme.service';
import { LanguageTypes } from '../../../cross-cutting/helper/i18n/model/languages.types';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { TranslatePipe } from '../../../cross-cutting/helper/i18n/translate.pipe';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';

@Component({
  selector: 'app-config-management',
  imports: [CommonModule, ...SHARED_STANDALONE_COMPONENTS, TranslatePipe],
  standalone: true,
  templateUrl: './config-management.component.html',
  styleUrl: './config-management.component.scss'
})
export class ConfigManagementComponent {
  TranslationCodes = TranslationCodes;

  constructor(private themeService: ThemeService, private i18nService: I18nService) { }

  toggleTheme() {
    this.themeService.toggleTheme();
  }

  setTheme(theme: 'light' | 'dark') {
    this.themeService.setTheme(theme);
  }

  setSystemTheme() {
    this.themeService.setSystemTheme();
  }

  get currentTheme(): 'light' | 'dark' {
    return this.themeService.getCurrentTheme();
  }

  changeLanguageToEnglish(): void {
    this.i18nService.setLanguage(LanguageTypes.en_US);
  }

  changeLanguageToSpanish(): void {
    this.i18nService.setLanguage(LanguageTypes.es_MX);
  }



}
