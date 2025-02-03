import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { I18nService } from '../cross-cutting/helper/i18n/i18n.service';
import { AuthenticationService } from '../cross-cutting/security/authentication/authentication.service';
import { CommonModule } from '@angular/common';
import { LoggingService } from '../cross-cutting/operation-management/logginService/logging.service';
import { TranslationCodes } from '../cross-cutting/helper/i18n/model/translation-codes.types';
import { LanguageTypes } from '../cross-cutting/helper/i18n/model/languages.types';
import { NotificationService } from '../cross-cutting/communication/notification-service/notification.service';
import { OperationResult } from '../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse } from '../cross-cutting/communication/model/api-response.error';
import { MessageCodeType } from '../cross-cutting/communication/model/message-code.types';
import { of, switchMap } from 'rxjs';
import { ModalComponent } from '../view/ui-components/modals-and-dialogs/modal/modal.component';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { AppointmentNotificationComponent } from '../view/ui-components/notification/notification/appointment-notification/appointment-notification.component';


@Component({
  selector: 'app-root',
  imports: [
    RouterModule,
    CommonModule,
    MatDialogModule
  ],
  standalone: true,
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})

export class AppComponent {
  translationCodes = TranslationCodes;
  isAuthenticated: boolean = false;
  notificationMessages: string[] = [];


  constructor(private notificationService: NotificationService, private i18nService: I18nService, private loggingService: LoggingService, private authService: AuthenticationService, private router: Router) {
  }

  ngOnInit(): void {

    this.authService.isAuthenticated().subscribe({
      next: (authenticated: boolean) => {
        if (authenticated) {
          //this.getAllNotifications();
          this.getUnreadNotifications();
        }
        this.isAuthenticated = authenticated;
      },
      error: (err) => {
        this.loggingService.warn(err);
      }
    });

    this.i18nService.setLanguage(this.i18nService.getLanguage());
  }



  logout(): void {
    this.authService.logout();
    //this.router.navigate([WebRoutes.login]);
  }

  changeLanguageToEnglish(): void {
    this.i18nService.setLanguage(LanguageTypes.en_US);
  }

  changeLanguageToSpanish(): void {
    this.i18nService.setLanguage(LanguageTypes.es_MX);
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  getUnreadNotifications(): void {
    this.notificationService.getUnreadNotifications().subscribe(() => { })
  }

}
