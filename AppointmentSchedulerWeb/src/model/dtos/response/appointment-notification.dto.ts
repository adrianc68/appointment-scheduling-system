import { Expose, Transform } from "class-transformer";
import { AppointmentNotificationCodeType } from "../../../view-model/business-entities/types/appointment-notification-code.types";
import { NotificationDTO } from "./notification.dto";
import { parseStringToEnum } from "../../../cross-cutting/helper/enum-utils/enum.utils";
import { InvalidValueEnumValueException } from "../exceptions/invalid-enum.exception";

export class AppointmentNotificationDTO extends NotificationDTO {
  @Expose({ name: "Code" })
  @Transform(({ value }) => {
    let data = parseStringToEnum(AppointmentNotificationCodeType, value);
    if (data === null || data === undefined) {
      throw new InvalidValueEnumValueException(`Invalid NotificationType value casting: ${value}`);
    }
    return data;
  })
  code!: AppointmentNotificationCodeType;
  @Expose({ name: "AppointmentUuid" })
  appointmentUuid!: string;
}
