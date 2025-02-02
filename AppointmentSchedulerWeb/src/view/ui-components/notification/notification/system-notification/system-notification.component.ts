import { Component } from '@angular/core';
import { NotificationComponent } from '../notification/notification.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-system-notification',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './system-notification.component.html',
  styleUrl: './system-notification.component.scss'
})
export class SystemNotificationComponent extends NotificationComponent {



}

