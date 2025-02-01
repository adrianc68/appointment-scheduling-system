import { SystemNotificationCodeType } from "../../view-model/business-entities/types/system-notification-code.types";
import { SystemNotificationSeverityCode } from "../../view-model/business-entities/types/system-notification-severity-code.types";
import { NotificationDTO } from "./notification.dto";

export interface SystemNotificationDTO extends NotificationDTO {
  code: SystemNotificationCodeType,
  severity: SystemNotificationSeverityCode
}


