import { Expose, Transform, Type } from "class-transformer";
import { AppointmentStatusType } from "../../../view-model/business-entities/types/appointment-status.types";
import { parseStringToEnum } from "../../../cross-cutting/helper/enum-utils/enum.utils";
import { InvalidValueEnumValueException } from "../exceptions/invalid-enum.exception";

export class AppointmentDTO {
  @Expose({ name: "uuid" })
  uuid: string;

  @Expose({ name: "startTime" })
  startTime: string;

  @Expose({ name: "endTime" })
  endTime: string;

  @Expose({ name: "date" })
  date: string;

  @Expose({ name: "status" })
  @Transform(({ value }) => {
    let data = parseStringToEnum(AppointmentStatusType, value);
    if (data === null || data === undefined) {
      throw new InvalidValueEnumValueException(`Invalid AppointmentStatusType value casting: ${value}`);
    }
    return data;
  })
  status?: AppointmentStatusType;

  @Expose({ name: "totalCost" })
  totalCost?: number;

  @Expose({ name: "createdAt" })
  createdAt: Date;

  @Type(() => AssistantDTO)
  @Expose({name: "assistants"})
  assistants: AssistantDTO[];

  constructor(uuid: string, startTime: string, endTime: string, date: string, status: AppointmentStatusType, totalCost: number, createdAt: Date, assistants: AssistantDTO[]) {
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


class OccupiedTimeRangeDTO {
  @Expose({name: "startTime"})
  startTime: string;

  @Expose({name: "endTime"})
  endTime: string;

  constructor(startTime: string, endTime: string) {
    this.startTime = startTime;
    this.endTime = endTime;
  }
}

class AssistantDTO {
  @Expose({name: "name"})
  name: string;

  @Expose({name: "uuid"})
  uuid: string;

  @Type(() => OccupiedTimeRangeDTO)
  @Expose({name: "occupiedTimeRange"})
  occupiedTimeRange: OccupiedTimeRangeDTO;

  constructor(name: string, uuid: string, occupiedTimeRange: OccupiedTimeRangeDTO) {
    this.name = name;
    this.uuid = uuid;
    this.occupiedTimeRange = occupiedTimeRange;
  }
}
