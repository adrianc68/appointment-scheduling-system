import { Component } from '@angular/core';
import { NotificationComponent } from '../notification/notification.component';

@Component({
  selector: 'app-general-notification',
  imports: [],
  standalone: true,
  templateUrl: './general-notification.component.html',
  styleUrl: './general-notification.component.scss'
})
export class GeneralNotificationComponent extends NotificationComponent {

}
