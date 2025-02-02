import { Component, EventEmitter, Output } from '@angular/core';
import { SHARED_STANDALONE_COMPONENTS } from '../../shared-components';
import { CommonModule } from '@angular/common';
import { AuthenticationService } from '../../../../cross-cutting/security/authentication/authentication.service';
import { Observable } from 'rxjs';
import { AccountData } from '../../../../view-model/business-entities/account';
import { I18nService } from '../../../../cross-cutting/helper/i18n/i18n.service';
import { TranslationCodes } from '../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { LanguageTypes } from '../../../../cross-cutting/helper/i18n/model/languages.types';

@Component({
  selector: 'app-nav-bar',
  imports: [CommonModule, ...SHARED_STANDALONE_COMPONENTS],
  standalone: true,
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.scss'
})
export class NavBarComponent {
  //isAuthenticated: Observable<boolean>
  accountData: Observable<AccountData | null>;
  translationCodes = TranslationCodes;

  @Output() menuToggle = new EventEmitter<void>();

  constructor(private authService: AuthenticationService, private i18nService: I18nService) {
    this.accountData = this.authService.getAccountData();
  }

  toggleSidebar() {
    this.menuToggle.emit();
  }

  logout(): void {
    this.authService.logout();
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  changeLanguageToEnglish(): void {
    this.i18nService.setLanguage(LanguageTypes.en_US);
  }

  changeLanguageToSpanish(): void {
    this.i18nService.setLanguage(LanguageTypes.es_MX);
  }




}
