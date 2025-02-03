import { Expose, Transform } from "class-transformer";
import { SystemNotificationCodeType } from "../../../view-model/business-entities/types/system-notification-code.types";
import { SystemNotificationSeverityCodeType } from "../../../view-model/business-entities/types/system-notification-severity-code.types";
import { NotificationDTO } from "./notification.dto";
import { parseStringToEnum } from "../../../cross-cutting/helper/enum-utils/enum.utils";
import { InvalidValueEnumValueException } from "../exceptions/invalid-enum.exception";

export class SystemNotificationDTO extends NotificationDTO {
  @Expose({ name: "Code" })
  @Transform(({ value }) => {
    let data = parseStringToEnum(SystemNotificationCodeType, value);
    if (data === null && data === undefined) {
      throw new InvalidValueEnumValueException(`Invalid SystemNotificationCodeType value casting: ${value}`);
    }
    return data;
  })
  code!: SystemNotificationCodeType;

  @Expose({ name: "Severity" })
  @Transform(({ value }) => {
    let data = parseStringToEnum(SystemNotificationSeverityCodeType, value);
    if (data === null && data === undefined) {
      throw new InvalidValueEnumValueException(`Invalid SystemNotificationSeverityCode value casting: ${value}`);
    }
    return data;
  })
  severity!: SystemNotificationSeverityCodeType;
}


