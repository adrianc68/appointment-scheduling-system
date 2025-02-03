import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { NotificationService } from '../../../cross-cutting/communication/notification-service/notification.service';
import { NotificationBase } from '../../../view-model/business-entities/notification/notification-base';

@Component({
  selector: 'app-notification-management',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './notification-management.component.html',
  styleUrl: './notification-management.component.scss'
})
export class NotificationManagementComponent {

  notificationList: NotificationBase[] = [];

  constructor(private notificationService: NotificationService) {
    this.notificationService.getMessages().subscribe((response) => {
      if (response) {
        this.notificationList = [];
        response.map((notification: NotificationBase) => {
          this.notificationList.push(notification);
        });

        this.notificationList = this.notificationList.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
        console.log(this.notificationList);
      }
    });


  }

}
