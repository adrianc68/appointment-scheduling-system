import { Component } from '@angular/core';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';

@Component({
  selector: 'app-general-notification-card',
  imports: [],
  standalone: true,
  templateUrl: './general-notification-card.component.html',
  styleUrl: './general-notification-card.component.scss'
})
export class GeneralNotificationCardComponent extends NotificationComponent {

}
