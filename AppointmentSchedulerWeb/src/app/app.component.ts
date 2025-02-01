import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { I18nService } from '../cross-cutting/helper/i18n/i18n.service';
import { AuthenticationService } from '../cross-cutting/security/authentication/authentication.service';
import { CommonModule } from '@angular/common';
import { LoggingService } from '../cross-cutting/operation-management/logginService/logging.service';
import { WebRoutes } from '../cross-cutting/operation-management/model/web-routes.constants';
import { TranslationCodes } from '../cross-cutting/helper/i18n/model/translation-codes.types';
import { LanguageTypes } from '../cross-cutting/helper/i18n/model/languages.types';
import { SignalRService } from '../cross-cutting/communication/signalr-service/signalr.service';
import { NotificationService } from '../cross-cutting/communication/notification-service/notification.service';
import { OperationResult } from '../cross-cutting/communication/model/operation-result.response';
import { NotificationDTO } from '../model/dtos/notification.dto';
import { ApiDataErrorResponse, isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../cross-cutting/communication/model/api-response.error';
import { getStringEnumKeyByValue } from '../cross-cutting/helper/enum-utils/enum.utils';
import { MessageCodeType } from '../cross-cutting/communication/model/message-code.types';
import { Observable, of, switchMap } from 'rxjs';
import { AppNotification } from '../view-model/business-entities/notification/notification';


@Component({
  selector: 'app-root',
  imports: [
    RouterModule,
    CommonModule,
  ],
  standalone: true,
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  translationCodes = TranslationCodes;
  isAuthenticated: boolean = false;
  notificationMessages: string[] = [];


  constructor(private notificationService: NotificationService, private i18nService: I18nService, private loggingService: LoggingService, private authService: AuthenticationService, private router: Router) {
  }

  ngOnInit(): void {

    this.i18nService.setLanguage(this.i18nService.getLanguage());
    this.authService.isAuthenticated().subscribe({
      next: (authenticated: boolean) => {
        if (authenticated) {
          this.getAllNotifications();
          this.getUnreadNotifications();
        }
        this.isAuthenticated = authenticated;
      },
      error: (err) => {
        this.loggingService.warn(err);
      }
    });
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

  getAllNotifications(): void {
    this.notificationService.getAllNotifications().pipe(
      switchMap((response: OperationResult<AppNotification[], ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          //let code = getStringEnumKeyByValue(MessageCodeType, response.code);
        }
        return of(response);
      })
    ).subscribe(response => {
      console.log(response);
    })
  }

  getUnreadNotifications(): void {
    this.notificationService.getUnreadNotifications().pipe(
      switchMap((response: OperationResult<AppNotification[], ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          //let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          //this.systemMessage = code;
        }
        return of(response);
      })
    ).subscribe(response => {
      if (response.result) {
        //this.notificationService.markNotificationAsRead(response.result[0].uuid).subscribe((response) => {
        //  console.log(response);
        //});
      }
      console.log(response);
    })
  }





}
