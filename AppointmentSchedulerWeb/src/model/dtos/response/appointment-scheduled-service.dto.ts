import { Type } from "class-transformer";
import { AssistantDTO } from "../assistant.dto";
import { ScheduledServiceDTO } from "./scheduled-service.dto";

export class AppointmentScheduledServiceDTO {
  @Type(() => ScheduledServiceDTO)
  service!: ScheduledServiceDTO;

  @Type(() => AssistantDTO)
  assistant!: AssistantDTO;

  constructor(service: ScheduledServiceDTO, assistant: AssistantDTO) {
    this.service = service;
    this.assistant = assistant;
  }
}
