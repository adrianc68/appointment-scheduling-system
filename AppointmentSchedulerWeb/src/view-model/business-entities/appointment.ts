import { AppointmentStatusType } from "./types/appointment-status.types";

export class Appointment {
  uuid: string;
  startTime: string;
  endTime: string;
  date: string;
  status?: AppointmentStatusType;
  totalCost?: number;
  createdAt: Date;
  assistants: Assistant[];

  constructor(uuid: string, startTime: string, endTime: string, date: string, createdAt: Date, assistants: Assistant[], status?: AppointmentStatusType, totalCost?: number) {
    this.uuid = uuid;
    this.startTime = startTime;
    this.endTime = endTime;
    this.date = date;
    this.status = status;
    this.totalCost = totalCost;
    this.createdAt = createdAt;
    this.assistants = assistants;
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
