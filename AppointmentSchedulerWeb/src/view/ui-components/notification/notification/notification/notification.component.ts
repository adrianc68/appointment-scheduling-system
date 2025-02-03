import { Component, Input } from '@angular/core';
import { NotificationBase } from '../../../../../view-model/business-entities/notification/notification-base';

@Component({
  selector: 'app-notification',
  templateUrl: './notification.component.html',
  styleUrl: './notification.component.scss'
})
export abstract class NotificationComponent {
  @Input() data!: NotificationBase;
}
