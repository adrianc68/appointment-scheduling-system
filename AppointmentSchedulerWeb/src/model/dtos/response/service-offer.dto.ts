import { Expose, Transform } from "class-transformer";
import { parseStringToEnum } from "../../../cross-cutting/helper/enum-utils/enum.utils";
import { ServiceOfferStatusType } from "../../../view-model/business-entities/types/service-offer-status.types";
import { InvalidValueEnumValueException } from "../exceptions/invalid-enum.exception";

export class ServiceOfferDTO {
  @Expose({ name: "name" })
  name: string;

  @Expose({ name: "price" })
  price: number;

  @Expose({ name: "minutes" })
  minutes: number;

  @Expose({ name: "description" })
  description: string;

  @Expose({ name: "uuid" })
  uuid: string;


  @Expose({ name: "status" })
  @Transform(({ value }) => {
    let data = parseStringToEnum(ServiceOfferStatusType, value);
    if (data === null || data === undefined) {
      throw new InvalidValueEnumValueException(`Invalid ServiceOfferStatusType value casting: ${value}`);
    }
    return data;
  })
  status: ServiceOfferStatusType

  @Expose({ name: "assistant" })
  assistant: { name: string, uuid: string }

  constructor(name: string, price: number, minutes: number, description: string, uuid: string, status: ServiceOfferStatusType, assistant: { name: string, uuid: string }) {
    this.name = name;
    this.price = price;
    this.minutes = minutes;
    this.description = description;
    this.uuid = uuid;
    this.status = status;
    this.assistant = assistant;
  }
}
