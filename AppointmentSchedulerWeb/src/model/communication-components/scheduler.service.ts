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
import { ClientDTO } from "../dtos/client.dto";
import { Client } from "../../view-model/business-entities/client";
import { RoleType } from "../../view-model/business-entities/types/role.types";
import { Assistant } from "../../view-model/business-entities/assistant";
import { AppointmentScheduledService } from "../../view-model/business-entities/appointment-scheduled-service";
import { ScheduledService } from "../../view-model/business-entities/scheduled-service";
import { AppointmentStatusType } from "../../view-model/business-entities/types/appointment-status.types";

@Injectable({
  providedIn: 'root'
})

export class SchedulerService {
  private apiUrl: string;

  constructor(private httpServiceAdapter: HttpClientAdapter, private configService: ConfigService) {
    this.apiUrl = this.configService.getApiBaseUrl() + ApiVersionRoute.v1;
  }


  blockTimeRange(appointment: any): Observable<OperationResult<Date, ApiDataErrorResponse>> {
    return this.httpServiceAdapter.post<string>(`${this.apiUrl}${ApiRoutes.blockTimeRangeAppointment}`, appointment).pipe(
      map((response: ApiResponse<Date, ApiDataErrorResponse>) => {
        console.log(response);
        if (this.httpServiceAdapter.isSuccessResponse<Date>(response)) {
          return OperationResultService.createSuccess(response.data, response.message);
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
    )
  };


  registerAppointmentAsClient(appointment: any): Observable<OperationResult<Date, ApiDataErrorResponse>> {
    return this.httpServiceAdapter.post<string>(`${this.apiUrl}${ApiRoutes.registerAppointmentAsClient}`, appointment).pipe(
      map((response: ApiResponse<Date, ApiDataErrorResponse>) => {
        console.log(response);
        if (this.httpServiceAdapter.isSuccessResponse<Date>(response)) {
          return OperationResultService.createSuccess(response.data, response.message);
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
    )
  };


  registerAppointmentAsStaff(appointment: any): Observable<OperationResult<Date, ApiDataErrorResponse>> {
    return this.httpServiceAdapter.post<string>(`${this.apiUrl}${ApiRoutes.registerAppointmentAsStaff}`, appointment).pipe(
      map((response: ApiResponse<Date, ApiDataErrorResponse>) => {
        console.log(response);
        if (this.httpServiceAdapter.isSuccessResponse<Date>(response)) {
          return OperationResultService.createSuccess(response.data, response.message);
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
    )
  };



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
    return this.httpServiceAdapter.get<AppointmentDTO>(`${this.apiUrl}${ApiRoutes.getScheduledAppointments}?startDate=${startDate}&endDate=${endDate}`).pipe(
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

  getScheduledOrConfirmedAppointmentsAsStaff(startDate: string, endDate: string): Observable<OperationResult<Appointment[], ApiDataErrorResponse>> {
    return this.httpServiceAdapter.get<AppointmentDTO>(`${this.apiUrl}${ApiRoutes.getScheduledAppointmentsDetails}?startDate=${startDate}&endDate=${endDate}`).pipe(
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

  getScheduledAppointmentsDetails(startDate: string, endDate: string): Observable<OperationResult<Appointment[], ApiDataErrorResponse>> {

    return this.httpServiceAdapter.get<AppointmentDTO>(`${this.apiUrl}${ApiRoutes.getScheduledAppointments}?startDate=${startDate}&endDate=${endDate}`).pipe(
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


  registerAvailabilityTimeSlot(availabilitySlot: any) {
    return this.httpServiceAdapter.post<string>(`${this.apiUrl}${ApiRoutes.registerAvailabilityTimeSlot}`, availabilitySlot).pipe(
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

  //editAvailabilityTimeSlot(slot: AvailabilityTimeSlot) {
  //
  //  return this.httpServiceAdapter.put<AvailabilityTimeSlot>(`${this.apiUrl}${ApiRoutes.editAvailabilityTimeSlot}`, slot).pipe(
  //    map((response: ApiResponse<string, ApiDataErrorResponse>) => {
  //      if (this.httpServiceAdapter.isSuccessResponse<string>(response)) {
  //        const guid = response.data;
  //        return OperationResultService.createSuccess(guid, response.message);
  //      }
  //      return OperationResultService.createFailure(response.data, response.message);
  //    }),
  //    catchError((err) => {
  //      if (err instanceof HttpErrorResponse) {
  //        let codeError = MessageCodeType.UNKNOWN_ERROR;
  //        if (err.status == 500) {
  //          codeError = MessageCodeType.SERVER_ERROR;
  //        }
  //        return of(OperationResultService.createFailure<ApiDataErrorResponse>(err.error, codeError));
  //      }
  //      return throwError(() => err);
  //    })
  //  );
  //}
  //
  //
  //


  confirmAppointment(uuid: string) {
    return this.httpServiceAdapter.patch<boolean>(`${this.apiUrl}${ApiRoutes.confirmAppointment}`, { uuid: uuid }).pipe(
      map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<boolean>(response)) {
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

  finalizeAppointment(uuid: string) {
    return this.httpServiceAdapter.patch<boolean>(`${this.apiUrl}${ApiRoutes.finalizeAppointment}`, { uuid: uuid }).pipe(
      map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<boolean>(response)) {
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

  cancelAppointmentAsStaff(uuid: string) {
    return this.httpServiceAdapter.patch<boolean>(`${this.apiUrl}${ApiRoutes.cancelAppointmentAsStaff}`, { uuid: uuid }).pipe(
      map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<boolean>(response)) {
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

  cancelAppointmentAsClient(uuid: string) {
    return this.httpServiceAdapter.patch<boolean>(`${this.apiUrl}${ApiRoutes.cancelAppointmentAsClient}`, { uuid: uuid }).pipe(
      map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<boolean>(response)) {
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


  disableAvailabilityTimeSlot(uuid: string) {
    return this.httpServiceAdapter.patch<boolean>(`${this.apiUrl}${ApiRoutes.disableAvailabilityTimeSlot}`, { uuid: uuid }).pipe(
      map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<boolean>(response)) {
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

  deleteAvailabilityTimeSlot(uuid: string) {
    return this.httpServiceAdapter.delete<boolean>(`${this.apiUrl}${ApiRoutes.deleteAvailabilityTimeSlot}?uuid=${uuid}`).pipe(
      map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<boolean>(response)) {
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

  enableAvailabilityTimeSlot(uuid: string) {
    return this.httpServiceAdapter.patch<boolean>(`${this.apiUrl}${ApiRoutes.enableAvailabilityTimeSlot}`, { uuid: uuid }).pipe(
      map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<boolean>(response)) {
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




  disableAssignedService(uuid: string) {
    return this.httpServiceAdapter.patch<boolean>(`${this.apiUrl}${ApiRoutes.disableAssignedService}`, { uuid: uuid }).pipe(
      map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<boolean>(response)) {
          return OperationResultService.createSuccess(response.data, response.message);
        }
        return OperationResultService.createFailure(response.message);
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

  enabledAssignedService(uuid: string) {
    return this.httpServiceAdapter.patch<boolean>(`${this.apiUrl}${ApiRoutes.enableAssignedService}`, { uuid: uuid }).pipe(
      map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<boolean>(response)) {
          return OperationResultService.createSuccess(response.data, response.message);
        }
        return OperationResultService.createFailure(response.message);
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


  deleteAssignedService(uuid: string) {
    return this.httpServiceAdapter.patch<boolean>(`${this.apiUrl}${ApiRoutes.deleteAssignedService}`, { uuid: uuid }).pipe(
      map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<boolean>(response)) {
          return OperationResultService.createSuccess(response.data, response.message);
        }
        return OperationResultService.createFailure(response.message);
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
    const unavailable = dto.unavailableTimeSlots.map(
      uts => new UnavailableTimeSlot(
        new Date(uts.startDate),
        new Date(uts.endDate)
      )
    );

    return new AvailabilityTimeSlot(
      dto.uuid,
      new Date(dto.startDate),
      new Date(dto.endDate),
      dto.assistant,
      dto.status,
      unavailable
    );
  }

  private parseServiceOfferAvailable(dto: ServiceOfferDTO): ServiceOffer {
    let data = new ServiceOffer(dto.name, dto.price, dto.minutes, dto.description, dto.uuid, dto.status, dto.assistant);
    return data;
  }

  private parseAppointments(dto: AppointmentDTO): Appointment {
    const scheduledServices = dto.scheduledServices?.map(ss =>
      new AppointmentScheduledService(
        new ScheduledService(
          ss.service.name,
          ss.service.price,
          ss.service.minutes,
          ss.service.uuid,
          new Date(ss.service.startDate),
          new Date(ss.service.endDate)
        ),
        new Assistant(
          ss.assistant.uuid,
          ss.assistant.email,
          ss.assistant.phoneNumber,
          ss.assistant.username,
          ss.assistant.name,
          RoleType.ASSISTANT,
          ss.assistant.status
        )
      )
    ) ?? [];

    const client = dto.client ? this.mapClient(dto.client) : undefined;

    const rawStatus = (dto.status ?? '').toString().trim().toUpperCase();
    const status = Object.values(AppointmentStatusType).includes(rawStatus as AppointmentStatusType)
      ? rawStatus as AppointmentStatusType
      : undefined;

    return new Appointment(
      dto.uuid,
      new Date(dto.startDate),
      new Date(dto.endDate),
      new Date(dto.createdAt),
      status,
      dto.totalCost,
      scheduledServices,
      client
    );
  }


  private mapClient(dto?: ClientDTO): Client | undefined {
    if (!dto) return undefined;
    return new Client(
      dto.uuid,
      dto.email,
      dto.phoneNumber,
      dto.username,
      dto.name,
      RoleType.CLIENT,
      dto.status,
      dto.createdAt
    );
  }



}
