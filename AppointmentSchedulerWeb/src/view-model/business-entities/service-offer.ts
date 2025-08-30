import { ServiceOfferStatusType } from "./types/service-offer-status.types";
import { ServiceStatusType } from "./types/service-status.types";

export class ServiceOffer {
  name: string;
  price: number;
  minutes: number;
  description: string;
  uuid: string;
  status: ServiceOfferStatusType;
  assistant?: { name: string, uuid: string };
  serviceUuid?: string;
  serviceStatus?: ServiceStatusType;

  constructor(name: string, price: number, minutes: number, description: string, uuid: string, status: ServiceOfferStatusType, assistant?: { name: string, uuid: string }, serviceUuid?: string, serviceStatus?: ServiceStatusType) {
    this.name = name;
    this.price = price;
    this.minutes = minutes;
    this.description = description;
    this.uuid = uuid;
    this.status = status;
    this.assistant = assistant;
    this.serviceUuid = serviceUuid;
    this.serviceStatus = serviceStatus;
  }
}
