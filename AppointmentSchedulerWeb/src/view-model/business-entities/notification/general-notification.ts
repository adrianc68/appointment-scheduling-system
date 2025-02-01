import { GeneralNotificationCodeType } from "../types/general-notification-code.types";
import { NotificationBase } from "./notification";


export interface GeneralNotification extends NotificationBase {
  code: GeneralNotificationCodeType
}
