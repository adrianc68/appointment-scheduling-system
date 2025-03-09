import { Expose } from "class-transformer";

export class AsssitantServiceOfferDTO {
  @Expose({ name: "name" })
  name: string;

  @Expose({ name: "uuid" })
  uuid: string;

  constructor(name: string, uuid: string) {
    this.name = name;
    this.uuid = uuid;
  }
}
