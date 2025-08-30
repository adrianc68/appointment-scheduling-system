import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { ServiceOffer } from '../../../view-model/business-entities/service-offer';
import { Appointment } from '../../../view-model/business-entities/appointment';
import { SchedulerService } from '../../../model/communication-components/scheduler.service';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { Observable, of, switchMap } from 'rxjs';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse, isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { Client } from '../../../view-model/business-entities/client';
import { ClientService } from '../../../model/communication-components/client.service';
import { SlotDateRangePipe } from '../../../cross-cutting/helper/date-utils/slot-date-range.pipe';
import { ReadableTimePipe } from '../../../cross-cutting/helper/date-utils/readable-time.pipe';
import { ReadableDatePipe } from '../../../cross-cutting/helper/date-utils/readable-date.pipe';
import { DurationDatePipe } from '../../../cross-cutting/helper/date-utils/duration-date.pipe';

@Component({
  selector: 'app-register-appointment-as-staff',
  imports: [CommonModule, FormsModule, MatIconModule, SlotDateRangePipe, ReadableTimePipe, ReadableDatePipe, DurationDatePipe],
  templateUrl: './register-appointment-as-staff.component.html',
  styleUrl: './register-appointment-as-staff.component.scss'
})
export class RegisterAppointmentAsStaffComponent {

  systemMessage?: string = '';
  translationCodes = TranslationCodes
  servicesAvailable: ServiceOffer[] = [];
  scheduledAppointments: Appointment[] = [];
  clients: Client[] = [];
  selectedDate: string = new Date().toISOString().split("T")[0]; // YYYY-MM-DD
  selectedServicesOffer: ServiceOffer[] = [];


  constructor(private schedulerService: SchedulerService, private i18nService: I18nService, private logginService: LoggingService, private clientService: ClientService) {

    this.clientService.getClientList().pipe(
      switchMap((response: OperationResult<Client[], ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.clients = [...response.result!];
          this.clients.map(d => console.log(d));
          this.systemMessage = code;
          return of(true);
        } else {
          this.handleErrorResponse(response);
          return of(false);
        }
      })
    ).subscribe({
      next: (result) => {
        console.log(result);
        //if(result) {
        //  thos.setSuccessfulTask();
        //} else {
        //  this.setUn
        //}
      },
      error: (err) => {
        this.logginService.error(err);

      }
    })

  }


  openedSlots = new Set<number>();

  toggleSlot(index: number) {
    if (this.openedSlots.has(index)) {
      this.openedSlots.delete(index);
    } else {
      this.openedSlots.add(index);
    }
  }

  getTotalMinutes(): number {
    if (!this.selectedServicesOffer) return 0;
    return this.selectedServicesOffer.reduce((total, service) => total + service.minutes, 0);
  }

  getTotalPrice(): number {
    if (!this.selectedServicesOffer) return 0;
    return this.selectedServicesOffer.reduce((total, service) => total + service.price, 0);
  }

  selectServiceOffer(serviceOffer: ServiceOffer) {
    const exists = this.selectedServicesOffer.some(s => s.uuid === serviceOffer.uuid);
    if (!exists) {
      this.selectedServicesOffer.push(serviceOffer);

      const index = this.servicesAvailable.findIndex(service => service.uuid === serviceOffer.uuid);
      if (index !== -1) {
        this.servicesAvailable.splice(index, 1);
      }
    }
  }

  removeSelectedService(serviceOffer: ServiceOffer) {
    const index = this.selectedServicesOffer.findIndex(service => service.uuid === serviceOffer.uuid);
    if (index !== -1) {
      this.selectedServicesOffer.splice(index, 1);

      this.servicesAvailable.push(serviceOffer);
    }
  }
  //startDate: string = "2024-01-01";
  //endDate: string = "2026-01-01";


  loadAppointments(startDate: string, endDate: string): void {
    this.scheduledAppointments = [];

    this.schedulerService.getScheduledOrConfirmedAppointmentsAsStaff(startDate, endDate).pipe(
      switchMap((response: OperationResult<Appointment[], ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.scheduledAppointments = [...response.result!];
          this.scheduledAppointments.map(d => console.log(d));
          this.systemMessage = code;
          return of(true);
        } else {
          this.handleErrorResponse(response);
          return of(false);
        }
      })
    ).subscribe({
      next: (result) => {
        console.log(result);
      },
      error: (err) => {
        this.logginService.error(err);
      }
    });
  }



  blockTimeRange(): void {

    const payload = {
      date: this.selectedDate,
      clientUuid: this.selectedClient?.uuid,
      selectedServices: this.selectedServicesOffer.map(s => ({
        uuid: s.uuid,
        startTime: this.startTimes[s.uuid]
      }))
    }; this.schedulerService.blockTimeRange(payload).pipe(
      switchMap((response: OperationResult<Date, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          console.log(response);
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = response.result ? new Date(response.result).toISOString() : code;
          return of(true);
        } else {
          this.handleErrorResponse(response);
          return of(false);
        }
      })
    ).subscribe({
      next: (result) => {
        console.log(result);
      },
      error: (err) => {
        this.logginService.error(err);
      }
    }
    )
  }



  registerAppointmentAsClient(): void {

    const payload = {
      date: this.selectedDate,
      clientUuid: this.selectedClient?.uuid,
      selectedServices: this.selectedServicesOffer.map(s => ({
        uuid: s.uuid,
        startTime: this.startTimes[s.uuid]
      }))
    };

    this.schedulerService.registerAppointmentAsClient(payload).pipe(
      switchMap((response: OperationResult<Date, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          console.log(response);
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = response.result ? new Date(response.result).toISOString() : code;
          return of(true);
        } else {
          this.handleErrorResponse(response);
          return of(false);
        }
      })
    ).subscribe({
      next: (result) => {
        console.log(result);
      },
      error: (err) => {
        this.logginService.error(err);
      }
    }
    )
  }


  selectedClient?: Client;

  registerAppointmentAsStaff(): void {
    console.log("called");

    const payload = {
      date: this.selectedDate,
      clientUuid: this.selectedClient?.uuid,
      selectedServices: this.selectedServicesOffer.map(s => ({
        uuid: s.uuid,
        startTime: this.startTimes[s.uuid]
      }))
    };

    this.schedulerService.registerAppointmentAsStaff(payload).pipe(
      switchMap((response: OperationResult<Date, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          console.log(response);
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(true);
        } else {
          this.handleErrorResponse(response);
          return of(false);
        }
      })
    ).subscribe({
      next: (result) => {
        console.log(result);
      },
      error: (err) => {
        this.logginService.error(err);
      }
    }
    )
  }


  startTimes: { [uuid: string]: string } = {};
  getEarliestStartTime(): Date | null {
    if (!this.selectedServicesOffer || this.selectedServicesOffer.length === 0) return null;

    const times = this.selectedServicesOffer
      .map(s => this.startTimes[s.uuid]) // tomamos del mapa
      .filter(t => t)                     // eliminamos undefined/null
      .map(t => new Date(`${this.selectedDate}T${t}`)); // combinamos con la fecha del appointment

    if (times.length === 0) return null;

    return new Date(Math.min(...times.map(d => d.getTime())));
  }

  // Devuelve la hora de fin estimada sumando la duración total
  getEstimatedEndTime(): Date | null | undefined {
    const start = this.getEarliestStartTime();
    if (!start) return null;

    const totalMinutes = this.getTotalMinutes();
    const end = new Date(start.getTime());
    end.setMinutes(end.getMinutes() + totalMinutes);

    return end;
  }


  onDateChange(date: string) {
    this.selectedDate = date;
    this.getAvailableServices(date);
    console.log("SELECTEDATE")
    console.log(this.selectedDate);
    this.loadAppointments(date, date);

  }

  getAvailableServices(date: string) {
    this.schedulerService.getAvailableServices(date).pipe(
      switchMap((response: OperationResult<ServiceOffer[], ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);

          this.servicesAvailable = [...response.result!].sort((a, b) =>
            a.name.localeCompare(b.name)
          );

          //this.servicesAvailable.map(d => console.log(d));
          this.systemMessage = code;
          return of(true);
        } else {
          this.handleErrorResponse(response);
          return of(false);
        }
      })
    ).subscribe({
      next: (result) => console.log(result),
      error: (err) => this.logginService.error(err)
    });
  }


  private handleErrorResponse(response: OperationResult<any, ApiDataErrorResponse>): void {

    let code = getStringEnumKeyByValue(MessageCodeType, MessageCodeType.UNKNOWN_ERROR);
    console.log("handleErrorResponse");
    console.log(code);
    console.log(response);
    console.log(response.error);
    if (isGenericErrorResponse(response.error)) {
      code = this.translationCodes.TC_GENERIC_ERROR_CONFLICT;
      code = getStringEnumKeyByValue(MessageCodeType, response.error.message);
    } else if (isValidationErrorResponse(response.error)) {
      code = this.translationCodes.TC_VALIDATION_ERROR;
    } else if (isServerErrorResponse(response.error)) {
      code = getStringEnumKeyByValue(MessageCodeType, response.code);
    } else if (isEmptyErrorResponse(response.error)) {
      code = getStringEnumKeyByValue(MessageCodeType, response.code);
    }

    this.systemMessage = code;
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }



}
