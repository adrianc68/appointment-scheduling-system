import { Component, OnInit } from '@angular/core';
import { NotificationComponent } from '../notification/notification.component';
import { CommonModule } from '@angular/common';
import { SystemNotification } from '../../../../../view-model/business-entities/notification/system-notification';

@Component({
  selector: 'app-system-notification',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './system-notification.component.html',
  styleUrl: './system-notification.component.scss'
})
export class SystemNotificationComponent extends NotificationComponent implements OnInit {
  override data: SystemNotification = {} as SystemNotification;


  ngOnInit(): void {
    this.data = this.data as SystemNotification;
  }


}

