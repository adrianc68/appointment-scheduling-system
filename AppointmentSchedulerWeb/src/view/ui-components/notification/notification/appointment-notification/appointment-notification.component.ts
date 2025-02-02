import { Component } from '@angular/core';
import { NotificationComponent } from '../notification/notification.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-appointment-notification',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './appointment-notification.component.html',
  styleUrl: './appointment-notification.component.scss'
})
export class AppointmentNotificationComponent extends NotificationComponent {

}
