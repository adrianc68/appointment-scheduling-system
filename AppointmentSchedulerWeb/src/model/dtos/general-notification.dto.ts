import { GeneralNotificationCodeType } from "../../view-model/business-entities/types/general-notification-code.types";
import { NotificationDTO } from "./notification.dto";

export interface GeneralNotificationDTO extends NotificationDTO {
  code: GeneralNotificationCodeType
}
