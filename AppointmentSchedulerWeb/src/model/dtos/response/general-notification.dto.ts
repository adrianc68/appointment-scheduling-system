import { Expose, Transform } from "class-transformer";
import { GeneralNotificationCodeType } from "../../../view-model/business-entities/types/general-notification-code.types";
import { NotificationDTO } from "./notification.dto";
import { parseStringToEnum } from "../../../cross-cutting/helper/enum-utils/enum.utils";
import { InvalidValueEnumValueException } from "../exceptions/invalid-enum.exception";

export class GeneralNotificationDTO extends NotificationDTO {
  @Expose({ name: "Code" })
  @Transform(({ value }) => {
    let data = parseStringToEnum(GeneralNotificationCodeType, value);
    if (data === null && data === undefined) {
      throw new InvalidValueEnumValueException(`Invalid GeneralNotificationCodeType value casting: ${value}`);
    }
    return data;
  })
  code!: GeneralNotificationCodeType
}
