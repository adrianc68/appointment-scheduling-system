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
import { CalendarComponent } from '../../ui-components/display/calendar/calendar.component';
import { AvailabilityTimeSlot } from '../../../view-model/business-entities/availability-time-slot';
import { fromLocalToUTC } from '../../../cross-cutting/helper/date-utils/date.utils';

@Component({
  selector: 'app-register-appointment-as-staff',
  imports: [CommonModule, FormsModule, MatIconModule, SlotDateRangePipe, ReadableTimePipe, ReadableDatePipe, DurationDatePipe, CalendarComponent],
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

  startTimes: { [uuid: string]: string } = {};
  selectedClient?: Client;
  slots: { startDate: string, endDate: string }[] = [];

  constructor(private schedulerService: SchedulerService, private i18nService: I18nService, private loggingService: LoggingService, private clientService: ClientService) {
    this.clientService.getClientList().pipe(
      switchMap((response: OperationResult<Client[], ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.clients = [...response.result!];
          this.systemMessage = code;
          return of(true);
        } else {
          this.handleErrorResponse(response);
          return of(false);
        }
      })
    ).subscribe();
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
    ).subscribe();
  }



  blockTimeRange(): void {
    console.log(this.selectedDate);

    const payload = {
      date: this.selectedDate,
      clientUuid: this.selectedClient?.uuid,
      selectedServices: this.selectedServicesOffer.map(s => ({
        uuid: s.uuid,
        startTime: this.startTimes[s.uuid]
      }))
    };
    console.log("BLOCKED TIME RANGE PAYLOAD");
    console.log(payload);

    console.log("BLOCKED TIME RANGE PAYLOAD");



    this.schedulerService.blockTimeRange(payload).pipe(
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
    ).subscribe();
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
    ).subscribe();
  }



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
    ).subscribe();
  }


  getEarliestStartTime(): Date | null {
    if (!this.selectedServicesOffer || this.selectedServicesOffer.length === 0) return null;

    const times = this.selectedServicesOffer
      .map(s => this.startTimes[s.uuid])
      .filter(t => t)
      .map(t => new Date(`${this.selectedDate}T${t}`));

    if (times.length === 0) return null;

    return new Date(Math.min(...times.map(d => d.getTime())));
  }

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
    this.loadAppointments(date, date);

  }

  onDateSelected(date: string) {
    this.selectedDate = date;
    this.loadAppointments(date, date);
    this.getAvailableServices(date);
  }



  onCurrentDateChange(date: Date) {
    const formatted = date.toLocaleString('es-MX', { month: 'long', year: 'numeric' });
    console.log('Mes actual:', formatted);

    // ejemplo: podrías calcular el rango del mes
    const start = new Date(date.getFullYear(), date.getMonth(), 1);
    const end = new Date(date.getFullYear(), date.getMonth() + 1, 0);
    console.log('Rango del mes:', start, end);

    //this.getAvailabilityTimeSlots(start.toString(), end.toString());

    this.getAvailabilityTimeSlots(
      this.formatDateToApi(start),
      this.formatDateToApi(end)
    );

  }

  private formatDateToApi(date: Date): string {
    const year = date.getUTCFullYear();
    const month = date.getUTCMonth() + 1; // enero = 0
    const day = date.getUTCDate();
    return `${year}-${month}-${day}`;
  }


  //slots: AvailabilityTimeSlot[] = [];

  getAvailabilityTimeSlots(startDate: string, endDate: string) {
    this.schedulerService.getAvailabilityTimeSlots(startDate, endDate).pipe(
      switchMap((response: OperationResult<AvailabilityTimeSlot[], ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          this.slots = response.result!.map(slot => ({
            startDate: new Date(slot.startDate).toISOString(),
            endDate: new Date(slot.endDate).toISOString()
          })); return of(true);
        } else {
          this.handleErrorResponse(response);
          return of(false);
        }
      })
    ).subscribe();
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
    ).subscribe();
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
