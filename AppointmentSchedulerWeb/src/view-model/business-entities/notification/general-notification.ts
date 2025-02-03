import { GeneralNotificationCodeType } from "../types/general-notification-code.types";
import { NotificationType } from "../types/notification.types";
import { NotificationBase } from "./notification-base";


export class GeneralNotification extends NotificationBase {
  constructor(
    createdAt: Date,
    uuid: string,
    message: string,
    type: NotificationType,
    public code: GeneralNotificationCodeType) {
    super(createdAt, uuid, message, type);
  }

}
