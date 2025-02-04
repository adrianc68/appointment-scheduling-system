import { Component } from '@angular/core';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';
import { SystemNotification } from '../../../../../view-model/business-entities/notification/system-notification';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-system-notification-card',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './system-notification-card.component.html',
  styleUrl: './system-notification-card.component.scss'
})
export class SystemNotificationCardComponent extends NotificationComponent {
  declare data: SystemNotification;

}
