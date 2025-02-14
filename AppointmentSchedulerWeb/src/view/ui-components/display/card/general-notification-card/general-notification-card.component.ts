import { Component } from '@angular/core';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';
import { GeneralNotification } from '../../../../../view-model/business-entities/notification/general-notification';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { ReadableDatePipe } from '../../../../../cross-cutting/helper/date-utils/readable-date.pipe';
import { TranslatePipe } from '../../../../../cross-cutting/helper/i18n/translate.pipe';

@Component({
  selector: 'app-general-notification-card',
  imports: [CommonModule, MatIconModule, ReadableDatePipe, TranslatePipe],
  standalone: true,
  templateUrl: './general-notification-card.component.html',
  styleUrl: './general-notification-card.component.scss'
})
export class GeneralNotificationCardComponent extends NotificationComponent {
  declare data: GeneralNotification;

  constructor() {
    super();
  }

}
