import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, filter, map, Observable, of, switchMap, throwError } from 'rxjs';
import { SignalRService } from '../signalr-service/signalr.service';
import { HttpClientAdapter } from '../http-client-adapter-service/http-client-adapter.service';
import { ConfigService } from '../../operation-management/configService/config.service';
import { OperationResult } from '../model/operation-result.response';
import { ApiDataErrorResponse } from '../model/api-response.error';
import { ApiRoutes, ApiVersionRoute } from '../../operation-management/model/api-routes.constants';
import { isAppointmentNotificationDTO, isGeneralNotificationDTO, isSystemNotificationDTO, NotificationDTO } from '../../../model/dtos/notification.dto';
import { ApiResponse } from '../../communication/model/api-response';
import { OperationResultService } from '../model/operation-result.service';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageCodeType } from '../model/message-code.types';
import { TranslationCodes } from '../../helper/i18n/model/translation-codes.types';
import { SystemNotification } from '../../../view-model/business-entities/notification/system-notification';
import { AppNotification } from '../../../view-model/business-entities/notification/notification';
import { AppointmentNotification } from '../../../view-model/business-entities/notification/appointment-notification';
import { GeneralNotification } from '../../../view-model/business-entities/notification/general-notification';
import { NotificationUuidDTO } from '../../../model/dtos/request/notification-uuid-request.dto';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private messagesSubject = new BehaviorSubject<string[]>([]);
  private isListenerAdded = false;
  translationCodes = TranslationCodes;

  constructor(private signalRService: SignalRService, private httpServiceAdapter: HttpClientAdapter, private configService: ConfigService) {
    this.signalRService.getConnectionStatus().pipe(
      filter(connected => connected),
      switchMap(() => {
        if (!this.isListenerAdded) {
          this.signalRService.addMessageListener("ReceiveNotification");
          this.isListenerAdded = true;
        }
        return this.signalRService.messageReceived;
      })
    ).subscribe((message: string) => {
      console.log("Notificación recibida:", message);
      this.messagesSubject.next([...this.messagesSubject.value, message]);
    });
  }

  getMessages(): Observable<string[]> {
    return this.messagesSubject.asObservable();
  }

  markNotificationAsRead(uuid: string): Observable<OperationResult<boolean, ApiDataErrorResponse>> {
    let requestDto = {
      Uuid: uuid
    } as NotificationUuidDTO

    return this.httpServiceAdapter.post<NotificationUuidDTO>(`${this.configService.getApiBaseUrl()}${ApiVersionRoute.v1}${ApiRoutes.markNotificationAsRead}`, requestDto).pipe(
      map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {
        console.log(response);
        if (this.httpServiceAdapter.isSuccessResponse<boolean>(response)) {
          return OperationResultService.createSuccess(response.data, response.message);
        }
        return OperationResultService.createFailure(response.data, response.message);
      }),
      catchError((err) => {
        if (err instanceof HttpErrorResponse) {
          let codeError = MessageCodeType.UNKNOWN_ERROR;
          if (err.status == 500) { // 500 < Http Status Code 500 Internal Server Error
            codeError = MessageCodeType.SERVER_ERROR;
          }
          return of(OperationResultService.createFailure<ApiDataErrorResponse>(err.error, codeError));
        }
        return throwError(() => err);
      })

    );
  }


  getUnreadNotifications(): Observable<OperationResult<AppNotification[], ApiDataErrorResponse>> {
    return this.httpServiceAdapter.get<NotificationDTO[]>(`${this.configService.getApiBaseUrl()}${ApiVersionRoute.v1}${ApiRoutes.getUnreadNotification}`).pipe(
      map((response: ApiResponse<NotificationDTO[], ApiDataErrorResponse>) => {
        console.log(response);
        if (this.httpServiceAdapter.isSuccessResponse<NotificationDTO[]>(response)) {
          const notifications: AppNotification[] = response.data.map(dto => this.mapNotificationDtoToNotification(dto));
          return OperationResultService.createSuccess(notifications, response.message);
        }
        return OperationResultService.createFailure([], response.message);
      }),
      catchError((err) => {
        if (err instanceof HttpErrorResponse) {
          let codeError = MessageCodeType.UNKNOWN_ERROR;
          if (err.status == 500) { // 500 < Http Status Code 500 Internal Server Error
            codeError = MessageCodeType.SERVER_ERROR;
          }
          return of(OperationResultService.createFailure<ApiDataErrorResponse>(err.error, codeError));
        }
        return throwError(() => err);
      })
    );
  }


  getAllNotifications(): Observable<OperationResult<AppNotification[], ApiDataErrorResponse>> {
    return this.httpServiceAdapter.get<NotificationDTO[]>(`${this.configService.getApiBaseUrl()}${ApiVersionRoute.v1}${ApiRoutes.getAllNotification}`).pipe(
      map((response: ApiResponse<NotificationDTO[], ApiDataErrorResponse>) => {
        console.log(response);
        if (this.httpServiceAdapter.isSuccessResponse<NotificationDTO[]>(response)) {
          const notifications: AppNotification[] = response.data.map(dto => this.mapNotificationDtoToNotification(dto));
          return OperationResultService.createSuccess(notifications, response.message);
        }
        return OperationResultService.createFailure([], response.message);
      }),
      catchError((err) => {
        if (err instanceof HttpErrorResponse) {
          let codeError = MessageCodeType.UNKNOWN_ERROR;
          if (err.status == 500) { // 500 < Http Status Code 500 Internal Server Error
            codeError = MessageCodeType.SERVER_ERROR;
          }
          return of(OperationResultService.createFailure<ApiDataErrorResponse>(err.error, codeError));
        }
        return throwError(() => err);
      })
    );
  }

  private mapNotificationDtoToNotification(dto: NotificationDTO): AppNotification {
    if (isSystemNotificationDTO(dto)) {
      return {
        ...dto,
        code: dto.code,
        severity: dto.severity
      } as SystemNotification;
    } else if (isAppointmentNotificationDTO(dto)) {
      return {
        ...dto,
        code: dto.code,
        appointment: dto.appointment
      } as AppointmentNotification;
    } else if (isGeneralNotificationDTO(dto)) {
      return {
        ...dto,
        code: dto.code
      } as GeneralNotification;
    } else {
      return dto as AppNotification;
    }
  }


}
