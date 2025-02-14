import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppointmentNotification } from '../../../../../view-model/business-entities/notification/appointment-notification';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';
import { TranslatePipe } from '../../../../../cross-cutting/helper/i18n/translate.pipe';
import { ReadableDatePipe } from '../../../../../cross-cutting/helper/date-utils/readable-date.pipe';

@Component({
  selector: 'app-appointment-notification-modal',
  imports: [CommonModule, TranslatePipe, ReadableDatePipe],
  standalone: true,
  templateUrl: './appointment-notification-modal.component.html',
  styleUrl: './appointment-notification-modal.component.scss'
})
export class AppointmentNotificationModalComponent extends NotificationComponent {
  declare data: AppointmentNotification;

  constructor() {
    super();

  }



}
