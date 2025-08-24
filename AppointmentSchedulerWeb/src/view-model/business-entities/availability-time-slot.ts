import { AvailabilityTimeSlotStatusType } from "./types/availability-time-slot-status.types";
import { UnavailableTimeSlot } from "./unavailable-time-slot";


export class AvailabilityTimeSlot {
  uuid: string;
  startDate: Date;
  endDate: Date;
  assistant: { name: string; uuid: string };
  status: AvailabilityTimeSlotStatusType;
  unavailableTimeSlots: UnavailableTimeSlot[];

  constructor(
    uuid: string,
    startDate: Date,
    endDate: Date,
    assistant: { name: string; uuid: string },
    status: AvailabilityTimeSlotStatusType,
    unavailableTimeSlots: UnavailableTimeSlot[]
  ) {
    this.uuid = uuid;
    this.startDate = startDate;
    this.endDate = endDate;
    this.assistant = assistant;
    this.status = status;
    this.unavailableTimeSlots = unavailableTimeSlots;
  }
}

