import { Expose } from "class-transformer";

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

  @Expose({ name: "assistant" })
  assistant: { name: string, uuid: string }

  constructor(name: string, price: number, minutes: number, description: string, uuid: string, assistant: { name: string, uuid: string }) {
    this.name = name;
    this.price = price;
    this.minutes = minutes;
    this.description = description;
    this.uuid = uuid;
    this.assistant = assistant;
  }
}
