import { Component } from '@angular/core';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';
import { AppointmentNotification } from '../../../../../view-model/business-entities/notification/appointment-notification';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-appointment-notification-card',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './appointment-notification-card.component.html',
  styleUrl: './appointment-notification-card.component.scss'
})
export class AppointmentNotificationCardComponent extends NotificationComponent {
  declare data: AppointmentNotification;


}
