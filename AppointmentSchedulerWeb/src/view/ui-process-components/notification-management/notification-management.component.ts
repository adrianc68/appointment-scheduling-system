import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { NotificationService } from '../../../cross-cutting/communication/notification-service/notification.service';
import { NotificationBase } from '../../../view-model/business-entities/notification/notification-base';
import { NotificationType } from '../../../view-model/business-entities/types/notification.types';
import { AppointmentNotificationCardComponent } from '../../ui-components/display/card/appointment-notification-card/appointment-notification-card.component';
import { SystemNotificationCardComponent } from '../../ui-components/display/card/system-notification-card/system-notification-card.component';
import { GeneralNotificationCardComponent } from '../../ui-components/display/card/general-notification-card/general-notification-card.component';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { NotificationStatusType } from '../../../view-model/business-entities/types/notification-status.types';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';

@Component({
  selector: 'app-notification-management',
  imports: [CommonModule, AppointmentNotificationCardComponent, SystemNotificationCardComponent, GeneralNotificationCardComponent, SHARED_STANDALONE_COMPONENTS],
  standalone: true,
  templateUrl: './notification-management.component.html',
  styleUrl: './notification-management.component.scss'
})
export class NotificationManagementComponent {
  notificationTypes = NotificationType;
  notificationList: NotificationBase[] = [];
  translationCodes = TranslationCodes;

  constructor(private notificationService: NotificationService, private i18nService: I18nService) {
    this.notificationService.getNotificationsObservable().subscribe((allResponse) => {
      if (allResponse) {
        const orderedList = [...allResponse];
        this.notificationList = orderedList.sort((a, b) => {
          return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
        });
      }
    });

    this.getAllNotificationsData();
  }

  castNotification<T extends NotificationBase>(notification: NotificationBase): T {
    return notification as T;
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  getAllNotificationsData(): void {
    this.notificationService.getAllNotifications().subscribe(() => { });
  }

  markNotificationAsRead(notification: NotificationBase) {
    this.notificationService.processUnreadNotification(notification);
  }


}
