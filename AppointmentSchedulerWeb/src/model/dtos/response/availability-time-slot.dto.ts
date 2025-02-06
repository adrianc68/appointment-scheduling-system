import { Expose, Transform } from "class-transformer";
import { AvailabilityTimeSlotStatusType } from "../../../view-model/business-entities/types/availability-time-slot-status.types";
import { parseStringToEnum } from "../../../cross-cutting/helper/enum-utils/enum.utils";
import { InvalidValueEnumValueException } from "../exceptions/invalid-enum.exception";
import { UnavailableTimeSlotDTO } from "../unavailable-time-slot.dto";

export class AvailabilityTimeSlotDTO {
  @Expose({ name: "uuid" })
  uuid: string;
  @Expose({ name: "date" })
  date: Date;
  @Expose({ name: "startTime" })
  startTime: string;
  @Expose({ name: "endTime" })
  endTime: string;
  @Expose({ name: "assistant" })
  assistant: { name: string, uuid: string }
  @Expose({ name: "status" })
  @Transform(({ value }) => {
    let data = parseStringToEnum(AvailabilityTimeSlotStatusType, value);
    if (data === null || data === undefined) {
      throw new InvalidValueEnumValueException(`Invalid AvailabilityTimeSlotStatusType value casting: ${value}`);
    }
    return data;
  })
  status: AvailabilityTimeSlotStatusType
  @Expose({ name: "unavailableTimeSlots"})
  unavailableTimeSlots: UnavailableTimeSlotDTO[];

  constructor(uuid: string, date: Date, startTime: string, endTime: string, assistant: { name: string, uuid: string }, status: AvailabilityTimeSlotStatusType, unavailableTimeSlots: UnavailableTimeSlotDTO[]) {
    this.uuid = uuid;
    this.date = date;
    this.startTime = startTime;
    this.endTime = endTime;
    this.assistant = assistant;
    this.status = status;
    this.unavailableTimeSlots = unavailableTimeSlots;
  }

}
