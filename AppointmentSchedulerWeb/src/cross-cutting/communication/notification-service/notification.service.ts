import { Injectable, OnInit, Type } from '@angular/core';
import { BehaviorSubject, catchError, filter, map, Observable, of, switchMap, tap, throwError } from 'rxjs';
import { SignalRService } from '../signalr-service/signalr.service';
import { HttpClientAdapter } from '../http-client-adapter-service/http-client-adapter.service';
import { ConfigService } from '../../operation-management/configService/config.service';
import { OperationResult } from '../model/operation-result.response';
import { ApiDataErrorResponse } from '../model/api-response.error';
import { ApiRoutes, ApiVersionRoute } from '../../operation-management/model/api-routes.constants';
import { ApiResponse } from '../../communication/model/api-response';
import { OperationResultService } from '../model/operation-result.service';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageCodeType } from '../model/message-code.types';
import { TranslationCodes } from '../../helper/i18n/model/translation-codes.types';
import { SystemNotification } from '../../../view-model/business-entities/notification/system-notification';
import { NotificationBase } from '../../../view-model/business-entities/notification/notification-base';
import { AppointmentNotification } from '../../../view-model/business-entities/notification/appointment-notification';
import { GeneralNotification } from '../../../view-model/business-entities/notification/general-notification';
import { NotificationUuidDTO } from '../../../model/dtos/request/notification-uuid-request.dto';
import { MatDialog } from '@angular/material/dialog';
import { ModalComponent } from '../../../view/ui-components/modals-and-dialogs/modal/modal.component';
import { NotificationComponent } from '../../../view/ui-components/notification/notification/notification/notification.component';
import { NotificationType } from '../../../view-model/business-entities/types/notification.types';
import { parseStringToEnum } from '../../helper/enum-utils/enum.utils';
import { plainToInstance } from 'class-transformer';
import { NotificationDTO } from '../../../model/dtos/response/notification.dto';
import { SystemNotificationDTO } from '../../../model/dtos/response/system-notification.dto';
import { GeneralNotificationDTO } from '../../../model/dtos/response/general-notification.dto';
import { AppointmentNotificationDTO } from '../../../model/dtos/response/appointment-notification.dto';
import { InvalidValueEnumValueException } from '../../../model/dtos/exceptions/invalid-enum.exception';
import { SystemNotificationModalComponent } from '../../../view/ui-components/modals-and-dialogs/modal/system-notification-modal/system-notification-modal.component';
import { GeneralNotificationModalComponent } from '../../../view/ui-components/modals-and-dialogs/modal/general-notification-modal/general-notification-modal.component';
import { AppointmentNotificationModalComponent } from '../../../view/ui-components/modals-and-dialogs/modal/appointment-notification-modal/appointment-notification-modal.component';
import { NotificationStatusType } from '../../../view-model/business-entities/types/notification-status.types';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private isListenerAdded = false;
  translationCodes = TranslationCodes;
  private isModalOpen = false;
  private notificationQueue: NotificationBase[] = [];
  private unreadNotificationsCount: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  //private unreadNotificationsList: BehaviorSubject<NotificationBase[] | null> = new BehaviorSubject<NotificationBase[] | null>([]);
  private allNotificationsList: BehaviorSubject<NotificationBase[] | null> = new BehaviorSubject<NotificationBase[] | null>([]);

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

    this.allNotificationsList.pipe(
      map((notifications) => {
        let unreadNotifications = notifications?.filter(n => n.status === NotificationStatusType.UNREAD);
        return unreadNotifications ? unreadNotifications.length : 0;
      })
    ).subscribe((count) => {
      this.unreadNotificationsCount.next(count);
    });

  }

  private receiveNotification(message: string): void {
    const mappedNotification = this.mapNotificationDtoStringToNotification(message);
    this.addToQueueAndShowModal(mappedNotification);
  }

  private addToQueueAndShowModal(notification: NotificationBase): void {
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
      this.showModalNotification(notification)
    }
  }

  showModalNotification(notification: NotificationBase): void {
    if (notification.type === NotificationType.SYSTEM_NOTIFICATION) {
      this.openModalComp(SystemNotificationModalComponent, notification)
    } else if (notification.type === NotificationType.GENERAL_NOTIFICATION) {
      this.openModalComp(GeneralNotificationModalComponent, notification)
    } else if (notification.type === NotificationType.APPOINTMENT_NOTIFICATION) {
      this.openModalComp(AppointmentNotificationModalComponent, notification)
    } else {
      throw Error("Unknown notification type");
    }
  }

  private openModalComp(component: Type<NotificationComponent>, data: NotificationBase): void {
    const dialogRef = this.dialog.open(ModalComponent, {
      data: {
        component: component,
        data: data,
      },
    });

    dialogRef.afterClosed().subscribe(() => {
      this.isModalOpen = false;
      this.markNotificationAsRead(data.uuid).subscribe((response) => {
        if (response.isSuccessful && response.result) {
          let currentNotifications = this.allNotificationsList.value || [];
          let updatedNotifications = currentNotifications.map(notification =>
            notification.uuid === data.uuid ? { ...notification, status: NotificationStatusType.READ } : notification
          );
          this.allNotificationsList.next(updatedNotifications);
        }
      });
      this.processQueue();
    });
  }

  getUnreadNotificationsObservable(): Observable<number> {
    return this.unreadNotificationsCount.asObservable();
  }

  getNotificationsObservable(): Observable<NotificationBase[] | null> {
    return this.allNotificationsList.asObservable();
  }

  markNotificationAsRead(uuid: string): Observable<OperationResult<boolean, ApiDataErrorResponse>> {
    let requestDto = {
      Uuid: uuid
    } as NotificationUuidDTO

    return this.httpServiceAdapter.post<NotificationUuidDTO>(`${this.configService.getApiBaseUrl()}${ApiVersionRoute.v1}${ApiRoutes.markNotificationAsRead}`, requestDto).pipe(
      map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {
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


  getUnreadNotifications(): Observable<OperationResult<boolean, ApiDataErrorResponse>> {
    return this.httpServiceAdapter.get<NotificationDTO[]>(`${this.configService.getApiBaseUrl()}${ApiVersionRoute.v1}${ApiRoutes.getUnreadNotification}`).pipe(
      map((response: ApiResponse<NotificationDTO[], ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<NotificationDTO[]>(response)) {
          const notifications: NotificationBase[] = response.data.map(dto => this.mapNotificationDTOToNotification(dto));
          this.allNotificationsList.next(notifications);
          return OperationResultService.createSuccess(true, response.message);
        }
        return OperationResultService.createFailure(false, response.message);
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


  getAllNotifications(): Observable<OperationResult<boolean, ApiDataErrorResponse>> {
    return this.httpServiceAdapter.get<NotificationDTO[]>(`${this.configService.getApiBaseUrl()}${ApiVersionRoute.v1}${ApiRoutes.getAllNotification}`).pipe(
      map((response: ApiResponse<NotificationDTO[], ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<NotificationDTO[]>(response)) {
          const notifications: NotificationBase[] = response.data.map(dto => this.mapNotificationDTOToNotification(dto));
          this.allNotificationsList.next(notifications);
          return OperationResultService.createSuccess(true, response.message);
        }
        return OperationResultService.createFailure(false, response.message);
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



  private mapNotificationDtoStringToNotification(message: string): NotificationBase {
    const rawObject = JSON.parse(message);
    let notificationType = parseStringToEnum(NotificationType, rawObject.Type);
    if (notificationType == NotificationType.GENERAL_NOTIFICATION) {
      let dto = plainToInstance(GeneralNotificationDTO, rawObject);
      return new GeneralNotification(dto.createdAt, dto.uuid, dto.message, dto.type, dto.status, dto.code);
    } else if (notificationType == NotificationType.SYSTEM_NOTIFICATION) {
      let dto = plainToInstance(SystemNotificationDTO, rawObject);
      return new SystemNotification(dto.createdAt, dto.uuid, dto.message, dto.type, dto.status, dto.code, dto.severity);
    } else if (notificationType == NotificationType.APPOINTMENT_NOTIFICATION) {
      let dto = plainToInstance(AppointmentNotificationDTO, rawObject);
      return new AppointmentNotification(dto.createdAt, dto.uuid, dto.message, dto.type, dto.status, dto.code, dto.appointmentUuid);
    }
    throw new InvalidValueEnumValueException("Cannot get notification type from json");
  }

  private mapNotificationDTOToNotification(notification: NotificationDTO): NotificationBase {
    let notificationType = notification.type;
    if (notificationType == NotificationType.GENERAL_NOTIFICATION) {
      let dto = notification as GeneralNotification;
      return new GeneralNotification(dto.createdAt, dto.uuid, dto.message, dto.type, dto.status, dto.code);
    } else if (notificationType == NotificationType.SYSTEM_NOTIFICATION) {
      let dto = notification as SystemNotification;
      return new SystemNotification(dto.createdAt, dto.uuid, dto.message, dto.type, dto.status, dto.code, dto.severity);
    } else if (notificationType == NotificationType.APPOINTMENT_NOTIFICATION) {
      let dto = notification as AppointmentNotification;
      return new AppointmentNotification(dto.createdAt, dto.uuid, dto.message, dto.type, dto.status, dto.code, dto.appointmentUuid);
    }
    throw new InvalidValueEnumValueException("Cannot get notification type from json");
  }


}
