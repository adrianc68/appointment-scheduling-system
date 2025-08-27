
import { Expose } from "class-transformer";

export class ScheduledServiceDTO {

  @Expose({ name: "minutes" })
  minutes: number;

  @Expose({ name: "name" })
  name: string;

  @Expose({ name: "price" })
  price: number;

  @Expose({ name: "uuid" })
  uuid: string;

  @Expose({ name: "startDate" })
  startDate: Date;

  @Expose({ name: "endDate" })
  endDate: Date;


  constructor(minutes: number, name: string, price: number, uuid: string, startDate: Date, endDate: Date) {
    this.minutes = minutes;
    this.name = name;
    this.price = price;
    this.uuid = uuid;
    this.startDate = startDate;
    this.endDate = endDate;
  }

}
