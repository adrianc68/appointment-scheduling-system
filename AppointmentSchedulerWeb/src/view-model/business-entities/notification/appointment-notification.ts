import { AppointmentUuidDTO } from "../../../model/dtos/appointment-uuid.dto";
import { AppointmentNotificationCodeType } from "../types/appointment-notification-code.types";
import { NotificationBase } from "./notification";

export interface AppointmentNotification extends NotificationBase {
  code: AppointmentNotificationCodeType,
  appointment: AppointmentUuidDTO
}
