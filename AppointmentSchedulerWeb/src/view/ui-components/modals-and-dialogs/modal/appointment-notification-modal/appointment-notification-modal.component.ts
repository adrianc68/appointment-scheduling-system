import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppointmentNotification } from '../../../../../view-model/business-entities/notification/appointment-notification';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';
import { TranslatePipe } from '../../../../../cross-cutting/helper/i18n/translate.pipe';
import { ReadableDatePipe } from '../../../../../cross-cutting/helper/date-utils/readable-date.pipe';
import { MatIconModule } from '@angular/material/icon';
import { ModalComponent } from '../modal.component';
import { MatDialogRef } from '@angular/material/dialog';
import { TranslationCodes } from '../../../../../cross-cutting/helper/i18n/model/translation-codes.types';

@Component({
  selector: 'app-appointment-notification-modal',
  imports: [CommonModule, TranslatePipe, ReadableDatePipe, MatIconModule],
  standalone: true,
  templateUrl: './appointment-notification-modal.component.html',
  styleUrl: './appointment-notification-modal.component.scss'
})
export class AppointmentNotificationModalComponent extends NotificationComponent {
  declare data: AppointmentNotification;
  TranslationCodes = TranslationCodes

  constructor(private dialogRef: MatDialogRef<ModalComponent>) {
    super();

  }

  close(): void {
    this.dialogRef.close();
  }



}
