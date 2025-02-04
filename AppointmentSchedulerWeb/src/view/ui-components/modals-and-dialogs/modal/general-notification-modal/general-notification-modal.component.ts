import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GeneralNotification } from '../../../../../view-model/business-entities/notification/general-notification';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';

@Component({
  selector: 'app-general-notification-modal',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './general-notification-modal.component.html',
  styleUrl: './general-notification-modal.component.scss'
})
export class GeneralNotificationModalComponent extends NotificationComponent {
  declare data: GeneralNotification;
}
