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
import { Router } from '@angular/router';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';
import { MatIconModule } from '@angular/material/icon';
import { SlotDateRangePipe } from '../../../cross-cutting/helper/date-utils/slot-date-range.pipe';

@Component({
  selector: 'app-appointment-management',
  imports: [CommonModule, FormsModule, ...SHARED_STANDALONE_COMPONENTS, ReadableDatePipe, MatIconModule, SlotDateRangePipe, ReadableDatePipe],
  standalone: true,
  templateUrl: './appointment-management.component.html',
  styleUrl: './appointment-management.component.scss'
})
export class AppointmentManagementComponent {
  systemMessage?: string = '';
  translationCodes = TranslationCodes
  servicesAvailable: ServiceOffer[] = [];
  scheduledAppointments: Appointment[] = [];


  constructor(private schedulerService: SchedulerService, private i18nService: I18nService, private logginService: LoggingService, private router: Router) {
  }


  openedSlots = new Set<number>();

  toggleSlot(index: number) {
    if (this.openedSlots.has(index)) {
      this.openedSlots.delete(index);
    } else {
      this.openedSlots.add(index);
    }
  }


  redirectToRegisterAppointment() {
    this.router.navigate([WebRoutes.appointment_management_register_as_staff]);
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
    ).subscribe({
      next: (result) => {
        console.log(result);
      },
      error: (err) => {
        this.logginService.error(err);
      }
    });
  }

  selectedDate: string = new Date().toISOString().split("T")[0];

  startDate: string = this.selectedDate;
  endDate: string = this.selectedDate;




  onDateRangeChange(value: string, type: 'start' | 'end') {
    if (type === 'start') {
      this.startDate = value;
    } else {
      this.endDate = value;
    }
    this.loadAppointments(this.startDate, this.endDate);
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
