import { Component, OnInit } from '@angular/core';
import { NotificationComponent } from '../notification/notification.component';
import { CommonModule } from '@angular/common';
import { GeneralNotification } from '../../../../../view-model/business-entities/notification/general-notification';

@Component({
  selector: 'app-general-notification',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './general-notification.component.html',
  styleUrl: './general-notification.component.scss'
})
export class GeneralNotificationComponent extends NotificationComponent implements OnInit {
  override data: GeneralNotification = {} as GeneralNotification;


  ngOnInit(): void {
    this.data = this.data as GeneralNotification;
  }


}
