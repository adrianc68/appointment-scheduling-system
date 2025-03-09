import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GeneralNotification } from '../../../../../view-model/business-entities/notification/general-notification';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';
import { TranslatePipe } from '../../../../../cross-cutting/helper/i18n/translate.pipe';
import { ReadableDatePipe } from '../../../../../cross-cutting/helper/date-utils/readable-date.pipe';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogRef } from '@angular/material/dialog';
import { ModalComponent } from '../modal.component';
import { TranslationCodes } from '../../../../../cross-cutting/helper/i18n/model/translation-codes.types';

@Component({
  selector: 'app-general-notification-modal',
  imports: [CommonModule, TranslatePipe, ReadableDatePipe, MatIconModule],
  standalone: true,
  templateUrl: './general-notification-modal.component.html',
  styleUrl: './general-notification-modal.component.scss'
})
export class GeneralNotificationModalComponent extends NotificationComponent {
  declare data: GeneralNotification;
  TranslationCodes = TranslationCodes;

  constructor(private dialogRef: MatDialogRef<ModalComponent>) {
    super();

  }

  close(): void {
    this.dialogRef.close();
  }
}
