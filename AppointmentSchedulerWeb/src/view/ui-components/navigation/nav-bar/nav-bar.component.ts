import { Component, EventEmitter, Output } from '@angular/core';
import { SHARED_STANDALONE_COMPONENTS } from '../../shared-components';
import { CommonModule } from '@angular/common';
import { AuthenticationService } from '../../../../cross-cutting/security/authentication/authentication.service';
import { Observable, takeUntil } from 'rxjs';
import { AccountData } from '../../../../view-model/business-entities/account';
import { I18nService } from '../../../../cross-cutting/helper/i18n/i18n.service';
import { TranslationCodes } from '../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { LanguageTypes } from '../../../../cross-cutting/helper/i18n/model/languages.types';
import { NotificationService } from '../../../../cross-cutting/communication/notification-service/notification.service';
import { WebRoutes } from '../../../../cross-cutting/operation-management/model/web-routes.constants';
import { Router } from '@angular/router';
import { Subject } from '@microsoft/signalr';

@Component({
  selector: 'app-nav-bar',
  imports: [CommonModule, ...SHARED_STANDALONE_COMPONENTS],
  standalone: true,
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.scss'
})
export class NavBarComponent {
  accountData: Observable<AccountData | null>;
  notificationsPendingNumber: number = 0;
  webRoutes = WebRoutes;

  translationCodes = TranslationCodes;

  @Output() menuToggle = new EventEmitter<void>();

  constructor(private router: Router, private authService: AuthenticationService, private i18nService: I18nService, private notificatinService: NotificationService) {
    this.accountData = this.authService.getAccountData();

    this.notificatinService.getUnreadNotificationsObservable().subscribe((count) => {
      this.notificationsPendingNumber = count;
    })

  }

  toggleSidebar() {
    this.menuToggle.emit();
  }

  logout(): void {
    this.authService.logout();
  }

  showNotifications(): void {
    this.router.navigate([WebRoutes.notifications]);
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
