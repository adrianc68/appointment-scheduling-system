import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';

@Component({
  selector: 'app-payment-notification-card',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './payment-notification-card.component.html',
  styleUrl: './payment-notification-card.component.scss'
})
export class PaymentNotificationCardComponent extends NotificationComponent implements OnInit {
  //override data: PaymentNotification = {} as PaymentNotification;

  ngOnInit(): void {
    //this.data = this.data as PaymentNotification;
  }



}
