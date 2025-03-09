import { Expose, Type } from "class-transformer";
import { ServiceOffer } from "../../../view-model/business-entities/service-offer";

export class AssistantServiceOfferDTO {
  @Expose({ name: "name" })
  name: string;

  @Expose({ name: "uuid" })
  uuid: string;

  constructor(name: string, uuid: string) {
    this.name = name;
    this.uuid = uuid;
  }
}

export class AssistantWithServiceOfferDTO {
  @Expose({ name: "assistant" })
  @Type(() => AssistantServiceOfferDTO)
  assistant: AssistantServiceOfferDTO;

  @Expose({ name: "serviceOffer" })
  @Type(() => ServiceOffer)
  serviceOffer: ServiceOffer[];

  constructor(assistant: AssistantServiceOfferDTO, serviceOffer: ServiceOffer[]) {
    this.assistant = assistant;
    this.serviceOffer = serviceOffer;
  }
}
