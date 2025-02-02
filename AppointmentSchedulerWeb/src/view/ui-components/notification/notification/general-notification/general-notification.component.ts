import { Component } from '@angular/core';
import { NotificationComponent } from '../notification/notification.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-general-notification',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './general-notification.component.html',
  styleUrl: './general-notification.component.scss'
})
export class GeneralNotificationComponent extends NotificationComponent {

}
