import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SystemNotification } from '../../../../../view-model/business-entities/notification/system-notification';
import { NotificationComponent } from '../../../notification/notification/notification/notification.component';
import { TranslationCodes } from '../../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { MatDialogRef } from '@angular/material/dialog';
import { ModalComponent } from '../modal.component';
import { ReadableDatePipe } from '../../../../../cross-cutting/helper/date-utils/readable-date.pipe';
import { MatIconModule } from '@angular/material/icon';
import { TranslatePipe } from '../../../../../cross-cutting/helper/i18n/translate.pipe';
import { SystemNotificationSeverityCodeType } from '../../../../../view-model/business-entities/types/system-notification-severity-code.types';

@Component({
  selector: 'app-system-notification',
  imports: [CommonModule, TranslatePipe, ReadableDatePipe, MatIconModule],
  standalone: true,
  templateUrl: './system-notification-modal.component.html',
  styleUrl: './system-notification-modal.component.scss'
})
export class SystemNotificationModalComponent extends NotificationComponent {
  declare data: SystemNotification;
  SeverityCodeTypes = SystemNotificationSeverityCodeType;
  TranslationCodes = TranslationCodes;

  constructor(private dialogRef: MatDialogRef<ModalComponent>) {
    super();
  }

  close(): void {
    this.dialogRef.close();
  }



}

