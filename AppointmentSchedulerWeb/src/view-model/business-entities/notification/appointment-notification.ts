import { AppointmentNotificationCodeType } from "../types/appointment-notification-code.types";
import { NotificationStatusType } from "../types/notification-status.types";
import { NotificationType } from "../types/notification.types";
import { NotificationBase } from "./notification-base";

export class AppointmentNotification extends NotificationBase {
  constructor(
    createdAt: Date,
    uuid: string,
    message: string,
    type: NotificationType,
    status: NotificationStatusType,
    public code: AppointmentNotificationCodeType,
    public appointmentUuid: string,
  ) {
    super(createdAt, uuid, message, type, status);
  }

}
