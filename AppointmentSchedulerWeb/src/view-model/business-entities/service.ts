import { ServiceStatusType } from "./types/service-status.types";

export class Service {
  description: string;
  minutes: number;
  name: string;
  price: number;
  uuid: string;
  status: ServiceStatusType;
  createdAt: Date

  constructor(description: string, minutes: number, name: string, price: number, uuid: string, status: ServiceStatusType, createdAt:Date) {
    this.description = description;
    this.minutes = minutes;
    this.name = name;
    this.price = price;
    this.uuid = uuid;
    this.status = status;
    this.createdAt = createdAt
  }

}
