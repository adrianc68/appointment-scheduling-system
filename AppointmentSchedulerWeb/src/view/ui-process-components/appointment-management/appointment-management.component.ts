import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { ServiceOffer } from '../../../view-model/business-entities/service-offer';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { SchedulerService } from '../../../model/communication-components/scheduler.service';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse, isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { Observable, of, switchMap } from 'rxjs';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { Appointment } from '../../../view-model/business-entities/appointment';
import { FormsModule } from '@angular/forms';
import { ReadableDatePipe } from '../../../cross-cutting/helper/date-utils/readable-date.pipe';
import { fromLocalToUTC } from '../../../cross-cutting/helper/date-utils/date.utils';

@Component({
  selector: 'app-appointment-management',
  imports: [CommonModule, FormsModule, ...SHARED_STANDALONE_COMPONENTS, ReadableDatePipe],
  standalone: true,
  templateUrl: './appointment-management.component.html',
  styleUrl: './appointment-management.component.scss'
})
export class AppointmentManagementComponent {
  systemMessage?: string = '';
  translationCodes = TranslationCodes
  servicesAvailable: ServiceOffer[] = [];


  constructor(private schedulerService: SchedulerService, private i18nService: I18nService, private logginService: LoggingService) {
  }


  //startDate: string = "2024-01-01";
  //endDate: string = "2026-01-01";
  scheduledAppointments: Appointment[] = [];


  loadAppointments(startDate: string, endDate: string): void {
    this.scheduledAppointments = [];

    this.schedulerService.getScheduledOrConfirmedAppointments(startDate, endDate).pipe(
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
      date: this.appointmentFormData.date,
      clientUuid: this.appointmentFormData.clientUuid,
      selectedServices: this.appointmentFormData.selectedServices.map(s => ({
        uuid: s.uuid,
        startTime: s.startTime
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

  appointmentFormData = {
    date: '',
    clientUuid: '',
    selectedServices: [
      {
        uuid: '',
        startTime: ''
      }
    ]
  };

  addService(): void {
    this.appointmentFormData.selectedServices.push({
      uuid: '',
      startTime: ''
    });
  }

  removeService(index: number): void {
    this.appointmentFormData.selectedServices.splice(index, 1);
  }

  registerAppointmentAsClient(): void {

    const payload = {
      date: this.appointmentFormData.date,
      clientUuid: this.appointmentFormData.clientUuid,
      selectedServices: this.appointmentFormData.selectedServices.map(s => ({
        uuid: s.uuid,
        startTime: s.startTime
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


  registerAppointmentAsStaff(): void {
    console.log("called");

    const payload = {
      date: this.appointmentFormData.date,
      clientUuid: this.appointmentFormData.clientUuid,
      selectedServices: this.appointmentFormData.selectedServices.map(s => ({
        uuid: s.uuid,
        startTime: s.startTime
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













  selectedDate: string = new Date().toISOString().split("T")[0]; // YYYY-MM-DD

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
