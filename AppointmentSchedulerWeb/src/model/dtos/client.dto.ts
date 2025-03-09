import { Expose, Transform } from "class-transformer";
import { AccountStatusType } from "../../view-model/business-entities/types/account-status.types";
import { parseStringToEnum } from "../../cross-cutting/helper/enum-utils/enum.utils";
import { InvalidValueEnumValueException } from "./exceptions/invalid-enum.exception";

export class ClientDTO {
  @Expose({ name: "email" })
  email!: string;

  @Expose({ name: "name" })
  name!: string;

  @Expose({ name: "phoneNumber" })
  phoneNumber!: string;

  @Expose({ name: "username" })
  username!: string;

  @Expose({ name: "status" })
  @Transform(({ value }) => {
    let data = parseStringToEnum(AccountStatusType, value);
    if (data === null || data === undefined) {
      throw new InvalidValueEnumValueException(`Invalid AccountStatusType value casting: ${value}`);
    }
    return data;
  })
  status!: AccountStatusType

  @Expose({ name: "createdAt" })
  createdAt!: Date

  @Expose({ name: "uuid" })
  uuid!: string

}
