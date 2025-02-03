import { NotificationType } from "../types/notification.types";
import { SystemNotificationCodeType } from "../types/system-notification-code.types";
import { SystemNotificationSeverityCodeType } from "../types/system-notification-severity-code.types";
import { NotificationBase } from "./notification-base";


export class SystemNotification extends NotificationBase {
  constructor(
    createdAt: Date,
    uuid: string,
    message: string,
    type: NotificationType,
    public code: SystemNotificationCodeType,
    public severity: SystemNotificationSeverityCodeType,
  ) {
    super(createdAt, uuid, message, type);
  }

}
