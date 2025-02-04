import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SystemNotification } from '../../../../../view-model/business-entities/notification/system-notification';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';

@Component({
  selector: 'app-system-notification',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './system-notification-modal.component.html',
  styleUrl: './system-notification-modal.component.scss'
})
export class SystemNotificationModalComponent extends NotificationComponent {
  declare data: SystemNotification;



}

