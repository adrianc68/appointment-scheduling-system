import { NotificationType } from "../types/notification.types";
import { AppointmentNotification } from "./appointment-notification";
import { GeneralNotification } from "./general-notification";
import { SystemNotification } from "./system-notification";

export interface NotificationBase {
  createdAt: Date,
  uuid: string,
  message: string,
  type: NotificationType
}


export type AppNotification = NotificationBase | GeneralNotification | SystemNotification | AppointmentNotification;

