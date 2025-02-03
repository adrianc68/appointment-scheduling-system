import { AppointmentNotificationCodeType } from "../types/appointment-notification-code.types";
import { NotificationType } from "../types/notification.types";
import { NotificationBase } from "./notification-base";

export class AppointmentNotification extends NotificationBase {
  constructor(
    createdAt: Date,
    uuid: string,
    message: string,
    type: NotificationType,
    public code: AppointmentNotificationCodeType,
    public appointmentUuid: string,
  ) {
    super(createdAt, uuid, message, type);
  }

}
