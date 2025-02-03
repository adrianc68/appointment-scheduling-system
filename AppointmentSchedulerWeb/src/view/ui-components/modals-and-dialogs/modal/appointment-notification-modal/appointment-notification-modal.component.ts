import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppointmentNotification } from '../../../../../view-model/business-entities/notification/appointment-notification';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';

@Component({
  selector: 'app-appointment-notification-modal',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './appointment-notification-modal.component.html',
  styleUrl: './appointment-notification-modal.component.scss'
})
export class AppointmentNotificationModalComponent extends NotificationComponent implements OnInit {

  override data: AppointmentNotification = {} as AppointmentNotification;

  ngOnInit(): void {
    this.data = this.data as AppointmentNotification;
  }




}
