import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { TaskStateManagerService } from '../../model/task-state-manager.service';
import { LoadingState } from '../../model/loading-state.type';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { catchError, map, Observable, of } from 'rxjs';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { UnavailableTimeSlot } from '../../../view-model/business-entities/unavailable-time-slot';
import { SchedulerService } from '../../../model/communication-components/scheduler.service';
import { fromLocalToUTC } from '../../../cross-cutting/helper/date-utils/date.utils';
import { AssistantService } from '../../../model/communication-components/assistant.service';
import { Assistant } from '../../../view-model/business-entities/assistant';
import { MatIconModule } from '@angular/material/icon';
import { ErrorUIService } from '../../../cross-cutting/communication/handle-error-service/error-ui.service';


@Component({
  selector: 'app-register-availability-time-slot',
  imports: [FormsModule, CommonModule, ...SHARED_STANDALONE_COMPONENTS, MatIconModule],
  standalone: true,
  providers: [TaskStateManagerService],
  templateUrl: './register-availability-time-slot.component.html',
  styleUrl: './register-availability-time-slot.component.scss'
})
export class RegisterAvailabilityTimeSlotComponent {

  translationCodes = TranslationCodes;

  startDate: Date = new Date();
  endDate: Date = new Date();
  assistantUuid: string = '';
  unavailableTimeSlots: UnavailableTimeSlot[] = [];
  assistants: Assistant[] = [];
  selectedAssistant?: Assistant;

  errorValidationMessage: { [field: string]: string[] } = {};
  systemMessage?: string = '';
  loadingState: LoadingState = LoadingState.NO_ACTION_PERFORMED;
  isSingleDay: boolean = true;
  currentStep = 1;
  previousStep = 1;

  slotDateMap: { [key: number]: string } = {};
  slotStartTimeMap: { [key: number]: string } = {};
  slotEndTimeMap: { [key: number]: string } = {};


  constructor(private i18nService: I18nService, private loggingService: LoggingService, private schedulerService: SchedulerService, private assistantService: AssistantService, private errorUIService: ErrorUIService) {

    this.getAssistantList();
  }



  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  startTime: string = '09:00';
  endTime: string = '17:00';
  selectedDate: string = new Date().toISOString().split('T')[0];


  nextStep() {
    this.currentStep++;
  }

  prevStep() {
    this.currentStep--;

    this.loadingState = LoadingState.NO_ACTION_PERFORMED;
  }


  ngAfterViewChecked(): void {
    if (this.previousStep !== this.currentStep) {
      window.scrollTo(0, 0);
      this.previousStep = this.currentStep;
    }
  }



  getStartEndDate(): { start: Date, end: Date } {


    if (this.isSingleDay) {
      const [startHour, startMin] = this.startTime.split(':').map(Number);
      const [endHour, endMin] = this.endTime.split(':').map(Number);
      const [year, month, day] = this.selectedDate.split('-').map(Number);
      const start = new Date(year, month - 1, day, startHour, startMin, 0, 0);
      const end = new Date(year, month - 1, day, endHour, endMin, 0, 0);

      return { start, end };
    } else {
      return { start: this.startDate, end: this.endDate };
    }
  }




  private getAssistantList(): void {
    this.assistantService.getAssistantList().pipe(
      map((response: OperationResult<Assistant[], ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK && response.result) {
          return response.result;
        } else {
          this.errorUIService.handleError(response);
          return [];
        }
      }),
      catchError(err => {
        this.loggingService.error(err);
        this.errorUIService.handleError(err);
        return of([]);
      })
    ).subscribe((assistants: Assistant[]) => {
      this.assistants = assistants;
    })
  }




  onSubmit() {
    if (this.loadingState === LoadingState.LOADING || this.loadingState == LoadingState.WORK_DONE) {
      return;
    }

    if (this.loadingState === LoadingState.SUCCESSFUL_TASK) {
      this.loadingState = LoadingState.WORK_DONE;
      return;
    }

    if (!this.selectedAssistant) {
      return;
    }

    this.loadingState = LoadingState.LOADING;
    this.systemMessage = "";
    this.errorValidationMessage = {};

    const { start, end } = this.getStartEndDate();

    const availabilitySlot = {
      StartDate: fromLocalToUTC(start),
      EndDate: fromLocalToUTC(end),
      AssistantUuid: this.selectedAssistant.uuid,
      UnavailableTimeSlots: this.getUnavailableSlotsUTC()

    };

    this.registerAvailabilityTimeSlot(availabilitySlot).subscribe(result => {
      if (result) {
        this.loadingState = LoadingState.SUCCESSFUL_TASK;
      } else {
        this.loadingState = LoadingState.UNSUCCESSFUL_TASK;
      }
    })
  }


  private registerAvailabilityTimeSlot(payload: any): Observable<boolean> {
    return this.schedulerService.registerAvailabilityTimeSlot(payload).pipe(
      map((response: OperationResult<string, ApiDataErrorResponse>): boolean => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
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
      }),
      catchError(err => {
        this.loggingService.error(err);
        return of(false);
      })
    )
  }


  private setErrorValidationMessage(key: string, value: string[]) {
    this.errorValidationMessage[key.toLowerCase()] = value;
  }


  addUnavailableTimeSlot() {
    this.unavailableTimeSlots.push({ startDate: new Date(), endDate: new Date() });
  }

  removeUnavailableTimeSlot(index: number) {
    this.unavailableTimeSlots.splice(index, 1);
  }

  getUnavailableSlotsUTC() {
    return this.unavailableTimeSlots
      .map((slot, i) => {
        if (this.isSingleDay) {
          const dateParts = this.selectedDate?.split('-').map(Number);
          const startParts = this.slotStartTimeMap[i]?.split(':').map(Number);
          const endParts = this.slotEndTimeMap[i]?.split(':').map(Number);

          if (!dateParts || dateParts.length < 3) return null;
          if (!startParts || startParts.length < 2) return null;
          if (!endParts || endParts.length < 2) return null;

          const [year, month, day] = dateParts;
          const [startHour, startMin] = startParts;
          const [endHour, endMin] = endParts;

          const startLocal = new Date(year, month - 1, day, startHour, startMin, 0, 0);
          const endLocal = new Date(year, month - 1, day, endHour, endMin, 0, 0);

          return { StartDate: fromLocalToUTC(startLocal), EndDate: fromLocalToUTC(endLocal) };
        } else {
          if (!slot.startDate || !slot.endDate) return null;
          return { StartDate: fromLocalToUTC(slot.startDate), EndDate: fromLocalToUTC(slot.endDate) };
        }
      })
      .filter((slot) => slot !== null);
  }

}
