import { Component } from '@angular/core';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';

@Component({
  selector: 'app-system-notification-card',
  imports: [],
  standalone: true,
  templateUrl: './system-notification-card.component.html',
  styleUrl: './system-notification-card.component.scss'
})
export class SystemNotificationCardComponent extends NotificationComponent {

}
