import { AppointmentScheduledService } from "./appointment-scheduled-service";
import { Client } from "./client";
import { AppointmentStatusType } from "./types/appointment-status.types";

export class Appointment {
  uuid: string;
  startDate: Date;
  endDate: Date;
  status?: AppointmentStatusType;
  totalCost?: number;
  createdAt: Date;
  scheduledServices?: AppointmentScheduledService[]
  client?: Client;

  constructor(uuid: string, startDate: Date, endDate: Date, createdAt: Date, status?: AppointmentStatusType, totalCost?: number, scheduledServices?: AppointmentScheduledService[], client?: Client) {
    this.uuid = uuid;
    this.startDate = startDate;
    this.endDate = endDate;
    this.status = status;
    this.totalCost = totalCost;
    this.createdAt = createdAt;
    //this.assistants = assistants;
    this.scheduledServices = scheduledServices;
    this.client = client;
  }
}


class OccupiedTimeRange {
  startTime: string;
  endTime: string;

  constructor(startTime: string, endTime: string) {
    this.startTime = startTime;
    this.endTime = endTime;
  }
}

class Assistant {
  name: string;
  uuid: string;
  occupiedTimeRange: OccupiedTimeRange;

  constructor(name: string, uuid: string, occupiedTimeRange: OccupiedTimeRange) {
    this.name = name;
    this.uuid = uuid;
    this.occupiedTimeRange = occupiedTimeRange;
  }
}
