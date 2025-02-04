import { Component } from '@angular/core';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';
import { GeneralNotification } from '../../../../../view-model/business-entities/notification/general-notification';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-general-notification-card',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './general-notification-card.component.html',
  styleUrl: './general-notification-card.component.scss'
})
export class GeneralNotificationCardComponent extends NotificationComponent {
  declare data: GeneralNotification;
}
