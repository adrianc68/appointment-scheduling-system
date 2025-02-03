import { NotificationType } from "../types/notification.types";

export class NotificationBase {
  constructor(
    public createdAt: Date,
    public uuid: string,
    public message: string,
    public type: NotificationType
  ) { }
}


