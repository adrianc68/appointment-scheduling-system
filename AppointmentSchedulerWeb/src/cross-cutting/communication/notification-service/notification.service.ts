import { Injectable, OnInit, Type } from '@angular/core';
import { BehaviorSubject, catchError, filter, finalize, map, Observable, of, switchMap, take, tap, throwError } from 'rxjs';
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
import { AppNotification, NotificationBase } from '../../../view-model/business-entities/notification/notification';
import { AppointmentNotification } from '../../../view-model/business-entities/notification/appointment-notification';
import { GeneralNotification } from '../../../view-model/business-entities/notification/general-notification';
import { NotificationUuidDTO } from '../../../model/dtos/request/notification-uuid-request.dto';
import { MatDialog } from '@angular/material/dialog';
import { ModalComponent } from '../../../view/ui-components/modals-and-dialogs/modal/modal.component';
import { NotificationComponent } from '../../../view/ui-components/notification/notification/notification/notification.component';
import { SystemNotificationComponent } from '../../../view/ui-components/notification/notification/system-notification/system-notification.component';
import { GeneralNotificationComponent } from '../../../view/ui-components/notification/notification/general-notification/general-notification.component';
import { NotificationType } from '../../../view-model/business-entities/types/notification.types';
import { AppointmentNotificationComponent } from '../../../view/ui-components/notification/notification/appointment-notification/appointment-notification.component';
import { SystemNotificationDTO } from '../../../model/dtos/system-notification.dto';
import { AppointmentNotificationDTO } from '../../../model/dtos/appointment-notification.dto';
import { GeneralNotificationDTO } from '../../../model/dtos/general-notification.dto';
import { getStringEnumKeyByValue, parseStringToEnum } from '../../helper/enum-utils/enum.utils';
import { AuthenticationService } from '../../security/authentication/authentication.service';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private messagesSubject = new BehaviorSubject<NotificationBase[]>([]);
  private isListenerAdded = false;
  translationCodes = TranslationCodes;
  private isModalOpen = false;
  private notificationQueue: any[] = [];

  constructor(private signalRService: SignalRService, private httpServiceAdapter: HttpClientAdapter, private configService: ConfigService, private dialog: MatDialog) {
    this.signalRService.getConnectionStatus().pipe(
      tap((connected) => {
        if (connected) {
          if (!this.isListenerAdded) {
            this.signalRService.addMessageListener("ReceiveNotification");
            this.isListenerAdded = true;
          }
        } else {
          this.isListenerAdded = false;
        }
      }),
      switchMap((connected) => {
        if (connected) {
          return this.signalRService.messageReceived;
        }
        return of(null);
      }),
      filter((message) => message !== null)
    ).subscribe((message: string) => {
      this.receiveNotification(message);
    });


  }


  private receiveNotification(message: string): void {
    const rawObject = JSON.parse(message);
    console.log(rawObject);
    //const mappedMessage = this.mapNotificationDtoToNotification(rawObject);
    //this.addToQueue(mappedMessage);
    this.addToQueue(rawObject);
  }

  private addToQueue(notification: any): void {
    this.notificationQueue.push(notification);
    this.processQueue();
  }

  private processQueue(): void {
    if (this.isModalOpen || this.notificationQueue.length === 0) {
      return;
    }

    this.isModalOpen = true;
    const notification = this.notificationQueue.shift();

    if (notification) {
      let dataType = parseStringToEnum(NotificationType, notification.Type);
      if (dataType === NotificationType.SYSTEM_NOTIFICATION) {
        this.openModalComp(SystemNotificationComponent, notification)
      } else if (dataType === NotificationType.GENERAL_NOTIFICATION) {
        this.openModalComp(GeneralNotificationComponent, notification)
      } else if (dataType === NotificationType.APPOINTMENT_NOTIFICATION) {
        this.openModalComp(AppointmentNotificationComponent, notification)
      } else {
        throw Error("Unknown notification type");
      }
    }
  }

  private openModalComp(component: Type<NotificationComponent>, data: any): void {
    const dialogRef = this.dialog.open(ModalComponent, {
      data: {
        component: component,
        data: data,
      },
    });

    dialogRef.afterClosed().subscribe(() => {
      this.isModalOpen = false;
      this.processQueue();
    });
  }














  //
  //getMessages(): Observable<NotificationBase[]> {
  //  return this.messagesSubject.asObservable();
  //}
  //
  //markNotificationAsRead(uuid: string): Observable<OperationResult<boolean, ApiDataErrorResponse>> {
  //  let requestDto = {
  //    Uuid: uuid
  //  } as NotificationUuidDTO
  //
  //  return this.httpServiceAdapter.post<NotificationUuidDTO>(`${this.configService.getApiBaseUrl()}${ApiVersionRoute.v1}${ApiRoutes.markNotificationAsRead}`, requestDto).pipe(
  //    map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {
  //      console.log(response);
  //      if (this.httpServiceAdapter.isSuccessResponse<boolean>(response)) {
  //        return OperationResultService.createSuccess(response.data, response.message);
  //      }
  //      return OperationResultService.createFailure(response.data, response.message);
  //    }),
  //    catchError((err) => {
  //      if (err instanceof HttpErrorResponse) {
  //        let codeError = MessageCodeType.UNKNOWN_ERROR;
  //        if (err.status == 500) { // 500 < Http Status Code 500 Internal Server Error
  //          codeError = MessageCodeType.SERVER_ERROR;
  //        }
  //        return of(OperationResultService.createFailure<ApiDataErrorResponse>(err.error, codeError));
  //      }
  //      return throwError(() => err);
  //    })
  //
  //  );
  //}


  //getUnreadNotifications(): Observable<OperationResult<AppNotification[], ApiDataErrorResponse>> {
  //  return this.httpServiceAdapter.get<NotificationDTO[]>(`${this.configService.getApiBaseUrl()}${ApiVersionRoute.v1}${ApiRoutes.getUnreadNotification}`).pipe(
  //    map((response: ApiResponse<NotificationDTO[], ApiDataErrorResponse>) => {
  //      console.log(response);
  //      if (this.httpServiceAdapter.isSuccessResponse<NotificationDTO[]>(response)) {
  //        const notifications: AppNotification[] = response.data.map(dto => this.mapNotificationDtoToNotification(dto));
  //        return OperationResultService.createSuccess(notifications, response.message);
  //      }
  //      return OperationResultService.createFailure([], response.message);
  //    }),
  //    catchError((err) => {
  //      if (err instanceof HttpErrorResponse) {
  //        let codeError = MessageCodeType.UNKNOWN_ERROR;
  //        if (err.status == 500) { // 500 < Http Status Code 500 Internal Server Error
  //          codeError = MessageCodeType.SERVER_ERROR;
  //        }
  //        return of(OperationResultService.createFailure<ApiDataErrorResponse>(err.error, codeError));
  //      }
  //      return throwError(() => err);
  //    })
  //  );
  //}


  //getAllNotifications(): Observable<OperationResult<AppNotification[], ApiDataErrorResponse>> {
  //  return this.httpServiceAdapter.get<NotificationDTO[]>(`${this.configService.getApiBaseUrl()}${ApiVersionRoute.v1}${ApiRoutes.getAllNotification}`).pipe(
  //    map((response: ApiResponse<NotificationDTO[], ApiDataErrorResponse>) => {
  //      console.log(response);
  //      if (this.httpServiceAdapter.isSuccessResponse<NotificationDTO[]>(response)) {
  //        const notifications: AppNotification[] = response.data.map(dto => this.mapNotificationDtoToNotification(dto));
  //        return OperationResultService.createSuccess(notifications, response.message);
  //      }
  //      return OperationResultService.createFailure([], response.message);
  //    }),
  //    catchError((err) => {
  //      if (err instanceof HttpErrorResponse) {
  //        let codeError = MessageCodeType.UNKNOWN_ERROR;
  //        if (err.status == 500) { // 500 < Http Status Code 500 Internal Server Error
  //          codeError = MessageCodeType.SERVER_ERROR;
  //        }
  //        return of(OperationResultService.createFailure<ApiDataErrorResponse>(err.error, codeError));
  //      }
  //      return throwError(() => err);
  //    })
  //  );
  //}



  //
  //private mapNotificationDtoToNotification(dto: any): AppNotification {
  //  // Mapeamos el DTO con las claves correctamente formateadas
  //  const normalizedDto: NotificationDTO = {
  //    createdAt: new Date(dto.CreatedAt), // Convertimos CreatedAt a createdAt
  //    uuid: dto.Uuid, // Convertimos Uuid a uuid
  //    message: dto.Message, // Convertimos Message a message
  //    type: dto.Type as NotificationType, // Convertimos Type a type
  //  };
  //
  //  // Determinar el tipo de notificación y hacer el mapeo en consecuencia
  //  switch (dto.Type) {
  //    case NotificationType.SYSTEM_NOTIFICATION: // Para SystemNotification
  //      return {
  //        ...normalizedDto,
  //        code: dto.Code, // Convertimos Code a code
  //        severity: dto.Severity, // Convertimos Severity a severity
  //      } as SystemNotification;
  //
  //    case NotificationType.APPOINTMENT_NOTIFICATION: // Para AppointmentNotification
  //      return {
  //        ...normalizedDto,
  //        code: dto.Code, // Convertimos Code a code
  //        appointment: dto.Appointment, // Convertimos Appointment a appointment
  //      } as AppointmentNotification;
  //
  //    case NotificationType.GENERAL_NOTIFICATION: // Para GeneralNotification
  //      return {
  //        ...normalizedDto,
  //        code: dto.Code, // Convertimos Code a code
  //      } as GeneralNotification;
  //
  //    default:
  //      throw new Error("Tipo de notificación desconocido");
  //  }
  //}
  //


  //
  //
  //private mapNotificationDtoToNotification(dto: any): AppNotification {
  //  const normalizedDto: NotificationDTO = {
  //    createdAt: new Date(dto.CreatedAt),
  //    uuid: dto.Uuid,
  //    message: dto.Message,
  //    type: dto.Type as NotificationType
  //  };
  //
  //  if (isSystemNotificationDTO(normalizedDto)) {
  //    return {
  //      ...normalizedDto,
  //      code: dto.Code,
  //      severity: dto.Severity
  //    } as SystemNotification;
  //  } else if (isAppointmentNotificationDTO(normalizedDto)) {
  //    return {
  //      ...normalizedDto,
  //      code: dto.Code,
  //      appointment: dto.Appointment
  //    } as AppointmentNotification;
  //  } else if (isGeneralNotificationDTO(normalizedDto)) {
  //    return {
  //      ...normalizedDto,
  //      code: dto.Code
  //    } as GeneralNotification;
  //  } else {
  //    throw Error("Not posibble to convert");
  //  }
  //}



}
