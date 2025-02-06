import { AvailabilityTimeSlotStatusType } from "./types/availability-time-slot-status.types";
import { UnavailableTimeSlot } from "./unavailable-time-slot";

export class AvailabilityTimeSlot {
  uuid: string;
  date: Date;
  startTime: string;
  endTime: string;
  assistant: { name: string, uuid: string };
  status: AvailabilityTimeSlotStatusType;
  unavailableTimeSlots: UnavailableTimeSlot[];
  constructor(uuid: string, date: Date, startTime: string, endTime: string, assistant: { name: string, uuid: string }, status: AvailabilityTimeSlotStatusType, unavailableTimeSlots: UnavailableTimeSlot[]) {
    this.uuid = uuid;
    this.date = date;
    this.startTime = startTime;
    this.endTime = endTime;
    this.assistant = assistant;
    this.status = status;
    this.unavailableTimeSlots = unavailableTimeSlots;
  }

}
