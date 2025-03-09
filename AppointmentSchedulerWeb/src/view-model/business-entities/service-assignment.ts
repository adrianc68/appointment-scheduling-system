import { ServiceOffer } from "./service-offer";

export class ServiceAssignment {
  serviceOffer: ServiceOffer[];
  assistantName: string;
  assistantUuid: string;

  constructor(serviceOffer: ServiceOffer[], assistantName: string, assistantUuid: string) {
    this.serviceOffer = serviceOffer;
    this.assistantName = assistantName;
    this.assistantUuid = assistantUuid;
  }
}
