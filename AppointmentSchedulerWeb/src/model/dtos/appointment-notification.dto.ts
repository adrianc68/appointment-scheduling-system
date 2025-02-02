import { AppointmentNotificationCodeType } from "../../view-model/business-entities/types/appointment-notification-code.types";
import { AppointmentUuidDTO } from "./appointment-uuid.dto";
import { NotificationDTO } from "./notification.dto";

export interface AppointmentNotificationDTO extends NotificationDTO {
  Code: AppointmentNotificationCodeType,
  Appointment: AppointmentUuidDTO
}
