import { Component } from '@angular/core';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';

@Component({
  selector: 'app-appointment-notification-card',
  imports: [],
  standalone: true,
  templateUrl: './appointment-notification-card.component.html',
  styleUrl: './appointment-notification-card.component.scss'
})
export class AppointmentNotificationCardComponent extends NotificationComponent {

}
