import { NotificationType } from "../../view-model/business-entities/types/notification.types";
import { AppointmentNotificationDTO } from "./appointment-notification.dto";
import { GeneralNotificationDTO } from "./general-notification.dto";
import { SystemNotificationDTO } from "./system-notification.dto";

export interface NotificationDTO {
  CreatedAt: Date,
  Uuid: string,
  Message: string,
  Type: NotificationType
}


export function isSystemNotificationDTO(notification: any): notification is SystemNotificationDTO {
  return (
    notification &&
    typeof notification === "object" &&
    "code" in notification &&
    "severity" in notification
  );
}


export function isAppointmentNotificationDTO(notification: any): notification is AppointmentNotificationDTO {
  return (
    notification &&
    typeof notification === "object" &&
    "code" in notification &&
    "appointment" in notification
  );
}

export function isGeneralNotificationDTO(notification: any): notification is GeneralNotificationDTO {
  return (
    notification &&
    typeof notification === "object" &&
    "code" in notification
  );
}



