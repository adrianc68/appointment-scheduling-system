import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { AvailabilityTimeSlot } from '../../../view-model/business-entities/availability-time-slot';
import { SchedulerService } from '../../../model/communication-components/scheduler.service';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { Observable, of, switchMap } from 'rxjs';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse, isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';
import { Router } from '@angular/router';
import { formatReadableDate, fromUTCtoLocal } from '../../../cross-cutting/helper/date-utils/date.utils';
import { ReadableDatePipe } from '../../../cross-cutting/helper/date-utils/readable-date.pipe';

@Component({
  selector: 'app-availability-time-slot-management',
  imports: [CommonModule, ...SHARED_STANDALONE_COMPONENTS, ReadableDatePipe],
  standalone: true,
  templateUrl: './availability-time-slot-management.component.html',
  styleUrl: './availability-time-slot-management.component.scss'
})
export class AvailabilityTimeSlotManagementComponent {
  systemMessage?: string = '';
  translationCodes = TranslationCodes;
  slots: AvailabilityTimeSlot[] = [];

  //constructor(private i18nService: I18nService, private router: Router) {}


  constructor(private schedulerService: SchedulerService, private i18nService: I18nService, private logginService: LoggingService, private router: Router) {
    this.schedulerService.getAvailabilityTimeSlots("2024-1-11", "2026-1-11").pipe(
      switchMap((response: OperationResult<AvailabilityTimeSlot[], ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.slots = [...response.result!];
          this.slots.map(d => console.log(d));
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


  private handleErrorResponse(response: OperationResult<AvailabilityTimeSlot[], ApiDataErrorResponse>): void {

    let code = getStringEnumKeyByValue(MessageCodeType, MessageCodeType.UNKNOWN_ERROR);
    if (isGenericErrorResponse(response.error)) {
      code = this.translationCodes.TC_GENERIC_ERROR_CONFLICT;
    } else if (isValidationErrorResponse(response.error)) {
      code = this.translationCodes.TC_VALIDATION_ERROR;
    } else if (isServerErrorResponse(response.error)) {
      code = getStringEnumKeyByValue(MessageCodeType, response.code);
    } else if (isEmptyErrorResponse(response.error)) {
      code = getStringEnumKeyByValue(MessageCodeType, response.code);
    }

    this.systemMessage = code;
  }
  //
  redirectToRegisterSlot() {
    this.router.navigate([WebRoutes.availability_time_slot_management_register_slot])
  }

  redirectToEditSlot(slot: AvailabilityTimeSlot) {
    this.router.navigate([WebRoutes.availability_time_slot_management_edit_slot], { state: { slot } });
  }
  //
  translate(key: string): string {
    return this.i18nService.translate(key);
  }


    fromUTCtoLocal = fromUTCtoLocal; // Exponerla al template
    formatReadable = formatReadableDate;






}
