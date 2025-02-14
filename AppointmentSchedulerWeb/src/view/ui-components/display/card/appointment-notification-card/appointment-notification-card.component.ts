import { Component } from '@angular/core';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';
import { AppointmentNotification } from '../../../../../view-model/business-entities/notification/appointment-notification';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { ReadableDatePipe } from '../../../../../cross-cutting/helper/date-utils/readable-date.pipe';
import { TranslatePipe } from '../../../../../cross-cutting/helper/i18n/translate.pipe';

@Component({
  selector: 'app-appointment-notification-card',
  imports: [CommonModule, MatIconModule, ReadableDatePipe, TranslatePipe],
  standalone: true,
  templateUrl: './appointment-notification-card.component.html',
  styleUrl: './appointment-notification-card.component.scss'
})
export class AppointmentNotificationCardComponent extends NotificationComponent {
  declare data: AppointmentNotification;

  constructor() {
    super();
  }


}
