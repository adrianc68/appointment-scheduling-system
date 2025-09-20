import { Component } from '@angular/core';
import { I18nService } from '../../../../../cross-cutting/helper/i18n/i18n.service';
import { TranslationCodes } from '../../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { WebRoutes } from '../../../../../cross-cutting/operation-management/model/web-routes.constants';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { SchedulerService } from '../../../../../model/communication-components/scheduler.service';
import { LoggingService } from '../../../../../cross-cutting/operation-management/logginService/logging.service';
import { Appointment } from '../../../../../view-model/business-entities/appointment';
import { OperationResult } from '../../../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse } from '../../../../../cross-cutting/communication/model/api-response.error';
import { catchError, map, Observable, of } from 'rxjs';
import { MessageCodeType } from '../../../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../../../cross-cutting/helper/enum-utils/enum.utils';
import { ErrorUIService } from '../../../../../cross-cutting/communication/handle-error-service/error-ui.service';
import { AppointmentStatusType } from '../../../../../view-model/business-entities/types/appointment-status.types';
import { ReadableTimePipe } from '../../../../../cross-cutting/helper/date-utils/readable-time.pipe';
import { ReadableDatePipe } from '../../../../../cross-cutting/helper/date-utils/readable-date.pipe';
import { SlotDateRangePipe } from '../../../../../cross-cutting/helper/date-utils/slot-date-range.pipe';
import { DurationDatePipe } from '../../../../../cross-cutting/helper/date-utils/duration-date.pipe';
import { ReadableDateTimePipe } from '../../../../../cross-cutting/helper/date-utils/readable-date-time.pipe';

@Component({
  selector: 'app-administrator-home',
  imports: [CommonModule, RouterModule, MatIconModule, ReadableTimePipe, ReadableDatePipe, SlotDateRangePipe, DurationDatePipe, ReadableDateTimePipe],
  standalone: true,
  templateUrl: './administrator-home.component.html',
  styleUrl: './administrator-home.component.scss'
})
export class AdministratorHomeComponent {
  translationCodes = TranslationCodes;
  webRoutes = WebRoutes

  selectedDate: string = new Date().toISOString().split("T")[0];
  scheduledAppointments: Appointment[] = [];
  startDate: string = this.selectedDate;
  endDate: string = this.selectedDate;
  today: Date = new Date();

  openedSlots = new Set<number>();
  openedSlotsNotToday = new Set<number>();

  constructor(private schedulerService: SchedulerService, private i18nService: I18nService, private logginService: LoggingService, private router: Router, private errorUIService: ErrorUIService) {
    const todayISO = this.today.toISOString().split("T")[0];

    this.startDate = todayISO;

    const future = new Date(this.today);
    future.setMonth(this.today.getMonth() + 1);
    this.endDate = future.toISOString().split("T")[0];

    this.loadAppointments(this.startDate, this.endDate);

  }

  toggleSlotNotToday(index: number) {
    if (this.openedSlotsNotToday.has(index)) {
      this.openedSlotsNotToday.delete(index);
    } else {
      this.openedSlotsNotToday.add(index);
    }
  }

  toggleSlot(index: number) {
    if (this.openedSlots.has(index)) {
      this.openedSlots.delete(index);
    } else {
      this.openedSlots.add(index);
    }
  }


  systemMessage?: string = '';

  errorValidationMessage: { [field: string]: string[] } = {};
  private setErrorValidationMessage(key: string, value: string[]) {
    this.errorValidationMessage[key.toLowerCase()] = value;
  }

  getAppointmentsForTodayUTC(): Appointment[] {
    const today = new Date();
    const startOfDayUTC = Date.UTC(today.getUTCFullYear(), today.getUTCMonth(), today.getUTCDate());
    const endOfDayUTC = Date.UTC(today.getUTCFullYear(), today.getUTCMonth(), today.getUTCDate() + 1);

    return this.scheduledAppointments.filter(app => {
      const appTime = new Date(app.startDate).getTime();
      return appTime >= startOfDayUTC && appTime < endOfDayUTC;
    });
  }

  getAppointmentsNotForTodayUTC(): Appointment[] {
    const today = new Date();
    const startOfDayUTC = Date.UTC(today.getUTCFullYear(), today.getUTCMonth(), today.getUTCDate());
    const endOfDayUTC = Date.UTC(today.getUTCFullYear(), today.getUTCMonth(), today.getUTCDate() + 1);

    return this.scheduledAppointments.filter(app => {
      const appTime = new Date(app.startDate).getTime();
      return appTime < startOfDayUTC || appTime >= endOfDayUTC;
    });
  }

  loadAppointments(startDate: string, endDate: string): void {
    this.scheduledAppointments = [];

    this.schedulerService.getScheduledOrConfirmedAppointmentsAsStaff(startDate, endDate).pipe(

      map((response: OperationResult<Appointment[], ApiDataErrorResponse>): boolean => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.scheduledAppointments = [...response.result!];
          this.scheduledAppointments = [...response.result!].sort((a, b) => {
            const dateA = new Date(a.startDate).getTime();
            const dateB = new Date(b.startDate).getTime();
            return dateA - dateB;
          });

          this.systemMessage = code;
          return true;
        } else {
          let code = this.errorUIService.handleError(response);
          this.systemMessage = code;
          const validationErrors = this.errorUIService.getValidationErrors(response);
          Object.entries(validationErrors).forEach(([field, messages]) => {
            this.setErrorValidationMessage(field, messages);
          });
          return false;
        }
      })
    ).subscribe();
  }


  cancelAppointment(uuid: string): void {
    const appointment = this.scheduledAppointments.find(a => a.uuid === uuid);

    if (!appointment || appointment.status === AppointmentStatusType.CANCELED) {
      return;
    }

    this.schedulerService.cancelAppointmentAsStaff(uuid).pipe(
      map((response: OperationResult<boolean, ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          appointment.status = AppointmentStatusType.CANCELED;
          return true;
        } else {
          this.errorUIService.handleError(response);
          const validationErrors = this.errorUIService.getValidationErrors(response);
          Object.entries(validationErrors).forEach(([field, messages]) => {
            this.setErrorValidationMessage(field, messages);
          });
          return false;
        }
      }),
      catchError(err => {
        this.logginService.error(err);
        return of(false);
      })
    ).subscribe()
  }

  finalizeAppointment(uuid: string): void {
    const appointment = this.scheduledAppointments.find(a => a.uuid === uuid);

    if (!appointment || appointment.status === AppointmentStatusType.FINISHED) {
      return;
    }

    this.schedulerService.finalizeAppointment(uuid).pipe(
      map((response: OperationResult<boolean, ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          appointment.status = AppointmentStatusType.FINISHED;
          return true;
        } else {
          this.errorUIService.handleError(response);
          const validationErrors = this.errorUIService.getValidationErrors(response);
          Object.entries(validationErrors).forEach(([field, messages]) => {
            this.setErrorValidationMessage(field, messages);
          });
          return false;
        }
      }),
      catchError(err => {
        this.logginService.error(err);
        return of(false);
      })
    ).subscribe()
  }

  confirmAppointment(uuid: string): void {
    const appointment = this.scheduledAppointments.find(a => a.uuid === uuid);

    if (!appointment || appointment.status === AppointmentStatusType.CONFIRMED) {
      return;
    }

    this.schedulerService.confirmAppointment(uuid).pipe(
      map((response: OperationResult<boolean, ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          appointment.status = AppointmentStatusType.CONFIRMED;
          return true;
        } else {
          this.errorUIService.handleError(response);
          const validationErrors = this.errorUIService.getValidationErrors(response);
          Object.entries(validationErrors).forEach(([field, messages]) => {
            this.setErrorValidationMessage(field, messages);
          });
          return false;
        }
      }),
      catchError(err => {
        this.logginService.error(err);
        return of(false);
      })
    ).subscribe()
  }




  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  getConfirmedAppointments(): number {
    return this.scheduledAppointments.filter(a => a.status === AppointmentStatusType.CONFIRMED).length;
  }

  getCancelledAppointments(): number {
    return this.scheduledAppointments.filter(a => a.status === AppointmentStatusType.CANCELED).length;
  }

  getScheduledAppointments(): number {
    return this.scheduledAppointments.filter(a => a.status === AppointmentStatusType.SCHEDULED).length;
  }




}
