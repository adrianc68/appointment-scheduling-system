import { SystemNotificationCodeType } from "../types/system-notification-code.types";
import { SystemNotificationSeverityCode } from "../types/system-notification-severity-code.types";
import { NotificationBase } from "./notification";


export interface SystemNotification extends NotificationBase {
  code: SystemNotificationCodeType,
  severity: SystemNotificationSeverityCode
}
