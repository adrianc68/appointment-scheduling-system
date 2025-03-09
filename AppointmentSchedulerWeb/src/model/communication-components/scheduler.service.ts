import { Injectable } from "@angular/core";
import { HttpClientAdapter } from "../../cross-cutting/communication/http-client-adapter-service/http-client-adapter.service";
import { ConfigService } from "../../cross-cutting/operation-management/configService/config.service";
import { ApiRoutes, ApiVersionRoute } from "../../cross-cutting/operation-management/model/api-routes.constants";
import { catchError, map, Observable, of, throwError } from "rxjs";
import { OperationResult } from "../../cross-cutting/communication/model/operation-result.response";
import { AvailabilityTimeSlot } from "../../view-model/business-entities/availability-time-slot";
import { ApiDataErrorResponse } from "../../cross-cutting/communication/model/api-response.error";
import { AvailabilityTimeSlotDTO } from "../dtos/response/availability-time-slot.dto";
import { ApiResponse } from "../../cross-cutting/communication/model/api-response";
import { OperationResultService } from "../../cross-cutting/communication/model/operation-result.service";
import { HttpErrorResponse } from "@angular/common/http";
import { MessageCodeType } from "../../cross-cutting/communication/model/message-code.types";
import { ServiceOffer } from "../../view-model/business-entities/service-offer";
import { ServiceOfferDTO } from "../dtos/response/service-offer.dto";
import { AppointmentDTO } from "../dtos/response/appointment.dto";
import { Appointment } from "../../view-model/business-entities/appointment";
import { UnavailableTimeSlot } from "../../view-model/business-entities/unavailable-time-slot";

@Injectable({
  providedIn: 'root'
})

export class SchedulerService {
  private apiUrl: string;

  constructor(private httpServiceAdapter: HttpClientAdapter, private configService: ConfigService) {
    this.apiUrl = this.configService.getApiBaseUrl() + ApiVersionRoute.v1;
  }

  getAvailabilityTimeSlots(startDate: string, endDate: string): Observable<OperationResult<AvailabilityTimeSlot[], ApiDataErrorResponse>> {

    return this.httpServiceAdapter.get<AvailabilityTimeSlotDTO[]>(`${this.apiUrl}${ApiRoutes.getAvailabilityTimeSlots}?startDate=${startDate}&endDate=${endDate}`).pipe(
      map((response: ApiResponse<AvailabilityTimeSlotDTO[], ApiDataErrorResponse>) => {
        console.log(response);
        if (this.httpServiceAdapter.isSuccessResponse<AvailabilityTimeSlotDTO[]>(response)) {
          const slots: AvailabilityTimeSlot[] = response.data.map(dto => this.parseSlot(dto));
          return OperationResultService.createSuccess(slots, response.message);
        }
        return OperationResultService.createFailure(response.data, response.message);
      }),
      catchError((err) => {
        if (err instanceof HttpErrorResponse) {
          let codeError = MessageCodeType.UNKNOWN_ERROR;
          if (err.status == 500) {
            codeError = MessageCodeType.SERVER_ERROR;
          }
          return of(OperationResultService.createFailure<ApiDataErrorResponse>(err.error, codeError));
        }
        return throwError(() => err);
      })
    );
  }

  getAvailableServices(date: string): Observable<OperationResult<ServiceOffer[], ApiDataErrorResponse>> {
    return this.httpServiceAdapter.get<ServiceOfferDTO>(`${this.apiUrl}${ApiRoutes.getAvailableServices}?date=${date}`).pipe(
      map((response: ApiResponse<ServiceOfferDTO[], ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<ServiceOfferDTO[]>(response)) {
          const servicesAvailable: ServiceOffer[] = response.data.map(dto => this.parseServiceOfferAvailable(dto));
          return OperationResultService.createSuccess(servicesAvailable, response.message);
        }
        return OperationResultService.createFailure(response.data, response.message);
      }),
      catchError((err) => {
        if (err instanceof HttpErrorResponse) {
          let codeError = MessageCodeType.UNKNOWN_ERROR;
          if (err.status == 500) {
            codeError = MessageCodeType.SERVER_ERROR;
          }
          return of(OperationResultService.createFailure<ApiDataErrorResponse>(err.error, codeError));
        }
        return throwError(() => err);
      })

    );
  }

  getScheduledOrConfirmedAppointments(startDate: string, endDate: string): Observable<OperationResult<Appointment[], ApiDataErrorResponse>> {
    return this.httpServiceAdapter.get<AppointmentDTO>(`${this.apiUrl}${ApiRoutes.getScheduledAppointments}?startDate${startDate}&endDate=${endDate}`).pipe(
      map((response: ApiResponse<AppointmentDTO[], ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<AppointmentDTO[]>(response)) {
          const appointments: Appointment[] = response.data.map(dto => this.parseAppointments(dto));
          return OperationResultService.createSuccess(appointments, response.message);
        }
        return OperationResultService.createFailure(response.data, response.message);
      }),
      catchError((err) => {
        if (err instanceof HttpErrorResponse) {
          let codeError = MessageCodeType.UNKNOWN_ERROR;
          if (err.status == 500) {
            codeError = MessageCodeType.SERVER_ERROR;
          }
          return of(OperationResultService.createFailure<ApiDataErrorResponse>(err.error, codeError));
        }
        return throwError(() => err);
      })
    );
  }


  registerAvailabilityTimeSlot(date: string, startTime: string, endTime: string, assistantUuid: string, unavailableTimeSlots: UnavailableTimeSlot[]) {
    const slotsData = {
      date: date,
      startTime: startTime,
      endTime: endTime,
      assistantUuid: assistantUuid,
      unavailableTimeSlots: unavailableTimeSlots
    };

    return this.httpServiceAdapter.post<string>(`${this.apiUrl}${ApiRoutes.registerAvailabilityTimeSlot}`, slotsData).pipe(
      map((response: ApiResponse<string, ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<string>(response)) {
          const guid = response.data;
          return OperationResultService.createSuccess(guid, response.message);
        }
        return OperationResultService.createFailure(response.data, response.message);
      }),
      catchError((err) => {
        if (err instanceof HttpErrorResponse) {
          let codeError = MessageCodeType.UNKNOWN_ERROR;
          if (err.status == 500) {
            codeError = MessageCodeType.SERVER_ERROR;
          }
          return of(OperationResultService.createFailure<ApiDataErrorResponse>(err.error, codeError));
        }
        return throwError(() => err);
      })
    );
  }

  editAvailabilityTimeSlot(slot: AvailabilityTimeSlot) {

    return this.httpServiceAdapter.put<AvailabilityTimeSlot>(`${this.apiUrl}${ApiRoutes.editAvailabilityTimeSlot}`, slot).pipe(
      map((response: ApiResponse<string, ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<string>(response)) {
          const guid = response.data;
          return OperationResultService.createSuccess(guid, response.message);
        }
        return OperationResultService.createFailure(response.data, response.message);
      }),
      catchError((err) => {
        if (err instanceof HttpErrorResponse) {
          let codeError = MessageCodeType.UNKNOWN_ERROR;
          if (err.status == 500) {
            codeError = MessageCodeType.SERVER_ERROR;
          }
          return of(OperationResultService.createFailure<ApiDataErrorResponse>(err.error, codeError));
        }
        return throwError(() => err);
      })
    );
  }


  private parseSlot(dto: AvailabilityTimeSlotDTO): AvailabilityTimeSlot {
    let data = new AvailabilityTimeSlot(dto.uuid, dto.date, dto.startTime, dto.endTime, dto.assistant, dto.status, dto.unavailableTimeSlots);
    return data;
  }

  private parseServiceOfferAvailable(dto: ServiceOfferDTO): ServiceOffer {
    let data = new ServiceOffer(dto.name, dto.price, dto.minutes, dto.description, dto.uuid, dto.status, dto.assistant);
    return data;
  }

  private parseAppointments(dto: AppointmentDTO): Appointment {
    let data = new Appointment(dto.uuid, dto.startTime, dto.endTime, dto.date, dto.createdAt, dto.assistants, dto.status, dto.totalCost);
    return data;
  }



}
