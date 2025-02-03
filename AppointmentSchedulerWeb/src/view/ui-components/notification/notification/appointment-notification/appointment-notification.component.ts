import { Component, OnInit } from '@angular/core';
import { NotificationComponent } from '../notification/notification.component';
import { CommonModule } from '@angular/common';
import { AppointmentNotification } from '../../../../../view-model/business-entities/notification/appointment-notification';

@Component({
  selector: 'app-appointment-notification',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './appointment-notification.component.html',
  styleUrl: './appointment-notification.component.scss'
})
export class AppointmentNotificationComponent extends NotificationComponent implements OnInit {

  override data: AppointmentNotification = {} as AppointmentNotification;


  ngOnInit(): void {
    this.data = this.data as AppointmentNotification;
  }




}
