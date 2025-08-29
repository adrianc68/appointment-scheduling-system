import { Expose, Transform, Type } from "class-transformer";
import { AppointmentStatusType } from "../../../view-model/business-entities/types/appointment-status.types";
import { parseStringToEnum } from "../../../cross-cutting/helper/enum-utils/enum.utils";
import { InvalidValueEnumValueException } from "../exceptions/invalid-enum.exception";
import { ScheduledService } from "../../../view-model/business-entities/scheduled-service";
import { ScheduledServiceDTO } from "./scheduled-service.dto";
import { ClientDTO } from "../client.dto";

export class AppointmentDTO {
  @Expose({ name: "uuid" })
  uuid: string;

  @Expose({ name: "startDate" })
  startDate: Date;

  @Expose({ name: "endDate" })
  endDate: Date;

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
  @Expose({ name: "assistants" })
  assistants: AssistantDTO[];

  @Type(() => ClientDTO)
  @Expose({ name: "client" })
  client?: ClientDTO;


  @Type(() => ScheduledServiceDTO)
  @Expose({ name: "selectedServices" })
  selectedServices?: ScheduledServiceDTO[]


  constructor(uuid: string, startDate: Date, endDate: Date, status: AppointmentStatusType, totalCost: number, createdAt: Date, assistants: AssistantDTO[], selectedServices: ScheduledServiceDTO[], client?: ClientDTO) {
    this.uuid = uuid;
    this.startDate = startDate;
    this.endDate = endDate;
    this.status = status;
    this.totalCost = totalCost;
    this.createdAt = createdAt;
    this.assistants = assistants;
    this.selectedServices = selectedServices;
    this.client = client;
  }
}


class OccupiedTimeRangeDTO {
  @Expose({ name: "startTime" })
  startTime: string;

  @Expose({ name: "endTime" })
  endTime: string;

  constructor(startTime: string, endTime: string) {
    this.startTime = startTime;
    this.endTime = endTime;
  }
}

class AssistantDTO {
  @Expose({ name: "name" })
  name: string;

  @Expose({ name: "uuid" })
  uuid: string;

  @Type(() => OccupiedTimeRangeDTO)
  @Expose({ name: "occupiedTimeRange" })
  occupiedTimeRange: OccupiedTimeRangeDTO;

  constructor(name: string, uuid: string, occupiedTimeRange: OccupiedTimeRangeDTO) {
    this.name = name;
    this.uuid = uuid;
    this.occupiedTimeRange = occupiedTimeRange;
  }
}
