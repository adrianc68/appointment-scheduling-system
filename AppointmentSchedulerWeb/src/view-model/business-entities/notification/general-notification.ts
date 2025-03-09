import { GeneralNotificationCodeType } from "../types/general-notification-code.types";
import { NotificationStatusType } from "../types/notification-status.types";
import { NotificationType } from "../types/notification.types";
import { NotificationBase } from "./notification-base";


export class GeneralNotification extends NotificationBase {
  constructor(
    createdAt: Date,
    uuid: string,
    message: string,
    type: NotificationType,
    status: NotificationStatusType,
    public code: GeneralNotificationCodeType) {
    super(createdAt, uuid, message, type, status);
  }

}
