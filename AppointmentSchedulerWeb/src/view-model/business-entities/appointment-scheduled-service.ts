import { Assistant } from "./assistant";
import { ScheduledService } from "./scheduled-service";

export class AppointmentScheduledService {
  service: ScheduledService;
  assistant: Assistant;

  constructor(service: ScheduledService, assistant: Assistant) {
    this.service = service;
    this.assistant = assistant;
  }
}

