import { Component } from '@angular/core';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';
import { SystemNotification } from '../../../../../view-model/business-entities/notification/system-notification';
import { CommonModule } from '@angular/common';
import { ReadableDatePipe } from '../../../../../cross-cutting/helper/date-utils/readable-date.pipe';
import { MatIconModule } from '@angular/material/icon';
import { SystemNotificationSeverityCodeType } from '../../../../../view-model/business-entities/types/system-notification-severity-code.types';
import { TranslatePipe } from '../../../../../cross-cutting/helper/i18n/translate.pipe';

@Component({
  selector: 'app-system-notification-card',
  imports: [CommonModule, MatIconModule, ReadableDatePipe, TranslatePipe],
  standalone: true,
  templateUrl: './system-notification-card.component.html',
  styleUrl: './system-notification-card.component.scss'
})
export class SystemNotificationCardComponent extends NotificationComponent {
  declare data: SystemNotification;
  SeverityCodeTypes = SystemNotificationSeverityCodeType;

  constructor() {
    super();
  }

  getClass(type: string): string {
    return {
      INFO: 'info',
      CRITICAL: 'critical',
      WARNING: 'warning'
    }[type] || '';
  }

}
