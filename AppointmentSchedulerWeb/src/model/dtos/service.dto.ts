import { Expose, Transform } from "class-transformer";
import { ServiceStatusType } from "../../view-model/business-entities/types/service-status.types";
import { parseStringToEnum } from "../../cross-cutting/helper/enum-utils/enum.utils";
import { InvalidValueEnumValueException } from "./exceptions/invalid-enum.exception";

export class ServiceDTO {
  @Expose({name: "description"})
  description: string;

  @Expose({name: "minutes"})
  minutes: number;

  @Expose({name: "name"})
  name: string;

  @Expose({name: "price"})
  price: number;

  @Expose({name: "uuid"})
  uuid: string;


  @Expose({name: "status"})
  @Transform(({ value }) => {
    let data = parseStringToEnum(ServiceStatusType, value);
    if (data === null || data === undefined) {
      throw new InvalidValueEnumValueException(`Invalid ServiceStatusType value casting: ${value}`);
    }
    return data;
  })
  status: ServiceStatusType;

  @Expose({ name: "createdAt" })
  createdAt: Date

  constructor(description: string, minutes: number, name: string, price: number, uuid: string, status: ServiceStatusType, createdAt: Date) {
    this.description = description;
    this.minutes = minutes;
    this.name = name;
    this.price = price;
    this.uuid = uuid;
    this.status = status;
    this.createdAt = createdAt
  }

}
