import { parseStringToEnum } from "../../../cross-cutting/helper/enum-utils/enum.utils";
import { NotificationType } from "../../../view-model/business-entities/types/notification.types";
import { InvalidValueEnumValueException } from "../exceptions/invalid-enum.exception";
import { Expose, Transform } from 'class-transformer';


export class NotificationDTO {
  @Expose({ name: "CreatedAt" })
  createdAt!: Date;

  @Expose({ name: "Uuid" })
  uuid!: string;

  @Expose({ name: "Message" })
  message!: string;

  @Expose({ name: "Type" })
  @Transform(({ value }) => {
    let data = parseStringToEnum(NotificationType, value);
    if (data === null && data === undefined) {
      throw new InvalidValueEnumValueException(`Invalid NotificationType value casting: ${value}`);
    }
    return data;
  })
  type!: NotificationType;

}


