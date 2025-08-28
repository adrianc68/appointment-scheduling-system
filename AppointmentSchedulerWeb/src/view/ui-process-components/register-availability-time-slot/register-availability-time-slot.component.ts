import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { TaskStateManagerService } from '../../model/task-state-manager.service';
import { LoadingState } from '../../model/loading-state.type';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse, isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { Observable, of, switchMap } from 'rxjs';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { UnavailableTimeSlot } from '../../../view-model/business-entities/unavailable-time-slot';
import { SchedulerService } from '../../../model/communication-components/scheduler.service';
import { fromLocalToUTC } from '../../../cross-cutting/helper/date-utils/date.utils';
import { AssistantService } from '../../../model/communication-components/assistant.service';
import { Assistant } from '../../../view-model/business-entities/assistant';
import { MatIconModule } from '@angular/material/icon';


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
  currentTaskState: LoadingState;

  isSingleDay: boolean = true; // por defecto asumimos un solo día


  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  startTime: string = '09:00';
  endTime: string = '17:00';
  selectedDate: string = new Date().toISOString().split('T')[0]; // "2025-08-27"


  onDateChange() {
    if (this.startDate && this.endDate) {
      this.isSingleDay = this.startDate.toDateString() === this.endDate.toDateString();
    } else {
      this.isSingleDay = true;
    }
  }


  getStartEndDate(): { start: Date, end: Date } {
    if (this.isSingleDay) {
      const [startHour, startMin] = this.startTime.split(':').map(Number);
      const [endHour, endMin] = this.endTime.split(':').map(Number);

      const start = new Date(this.selectedDate);
      start.setHours(startHour, startMin, 0, 0);

      const end = new Date(this.selectedDate);
      end.setHours(endHour, endMin, 0, 0);

      return { start, end };
    } else {
      return { start: this.startDate, end: this.endDate };
    }
  }



  constructor(private titleService: Title, private router: Router, private i18nService: I18nService, private loggingService: LoggingService, private schedulerService: SchedulerService, private stateManagerService: TaskStateManagerService, private assistantService: AssistantService) {
    this.currentTaskState = this.stateManagerService.getState();
    this.stateManagerService.getStateAsObservable().subscribe(state => { this.currentTaskState = state });

    this.assistantService.getAssistantList().pipe(
      switchMap((response: OperationResult<Assistant[], ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.assistants = [...response.result!];
          this.assistants.map(d => console.log(d));
          this.systemMessage = code;
          return of(true);
        } else {
          this.handleErrorResponseAssistant(response);
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
        this.loggingService.error(err);

      }
    })
  }

  private handleErrorResponseAssistant(response: OperationResult<Assistant[], ApiDataErrorResponse>): void {

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


  onSubmit() {
    if (!this.selectedAssistant) {
      return;
    }

    if (this.currentTaskState === LoadingState.LOADING) {
      return;
    }

    this.stateManagerService.setState(LoadingState.LOADING);
    this.systemMessage = "";
    this.errorValidationMessage = {};

    const { start, end } = this.getStartEndDate();


    const availabilitySlot = {
      StartDate: fromLocalToUTC(start),
      EndDate: fromLocalToUTC(end),
      AssistantUuid: this.selectedAssistant.uuid,
      UnavailableTimeSlots: this.getUnavailableSlotsUTC()

    };

    //console.log("Payload enviado:", availabilitySlot);

    this.schedulerService.registerAvailabilityTimeSlot(availabilitySlot).pipe(
      switchMap((response: OperationResult<string, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          return of(true);
        } else {
          this.handleErrorResponse(response);
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(false);
        }
      }),
    ).subscribe({
      next: (result) => {
        if (result) {
          this.setSuccessfulTask();
        } else {
          this.setUnsuccessfulTask(LoadingState.UNSUCCESSFUL_TASK);
        }
      },
      error: (err) => {
        this.loggingService.error(err);
        this.setUnsuccessfulTask(LoadingState.UNSUCCESSFUL_TASK);
      }
    });
  }

  private handleErrorResponse(response: OperationResult<string, ApiDataErrorResponse>): void {

    let code = getStringEnumKeyByValue(MessageCodeType, MessageCodeType.UNKNOWN_ERROR);
    if (isGenericErrorResponse(response.error)) {
      let codeMesasge = getStringEnumKeyByValue(MessageCodeType, response.error.message);
      if (response.error.additionalData?.["field"] !== undefined) {
        this.setErrorValidationMessage(response.error.additionalData["field"], [codeMesasge!]);
      }
      code = this.translationCodes.TC_GENERIC_ERROR_CONFLICT;
    } else if (isValidationErrorResponse(response.error)) {
      response.error.forEach(errorItem => {
        this.setErrorValidationMessage(errorItem.field, errorItem.messages);
      });
      code = this.translationCodes.TC_VALIDATION_ERROR;
    } else if (isServerErrorResponse(response.error)) {
      code = getStringEnumKeyByValue(MessageCodeType, response.code);
    } else if (isEmptyErrorResponse(response.error)) {
      code = getStringEnumKeyByValue(MessageCodeType, response.code);
    }

    this.systemMessage = code;
  }


  private setErrorValidationMessage(key: string, value: string[]) {
    this.errorValidationMessage[key.toLowerCase()] = value;
  }

  private setUnsuccessfulTask(state: LoadingState): void {
    this.stateManagerService.setState(state);
    setTimeout(() => {
      this.stateManagerService.setState(LoadingState.NO_ACTION_PERFORMED);
    }, 1500);
  }

  private setSuccessfulTask(): void {
    this.stateManagerService.setState(LoadingState.SUCCESSFUL_TASK);
    setTimeout(() => {
      //this.router.navigate([WebRoutes.login])
    }, 1500)
  }

  addUnavailableTimeSlot() {
    this.unavailableTimeSlots.push({ startDate: new Date(), endDate: new Date() });
  }

  removeUnavailableTimeSlot(index: number) {
    this.unavailableTimeSlots.splice(index, 1);
  }


  slotDateMap: { [key: number]: string } = {};
  slotStartTimeMap: { [key: number]: string } = {};
  slotEndTimeMap: { [key: number]: string } = {};


  getUnavailableSlotsUTC() {
    return this.unavailableTimeSlots.map((slot, i) => {
      if (this.isSingleDay) {
        const [year, month, day] = this.selectedDate.split('-').map(Number);
        const [startHour, startMin] = this.slotStartTimeMap[i].split(':').map(Number);
        const [endHour, endMin] = this.slotEndTimeMap[i].split(':').map(Number);

        // Creamos un Date local combinando fecha + hora del slot
        const startLocal = new Date(year, month - 1, day, startHour, startMin, 0, 0);
        const endLocal = new Date(year, month - 1, day, endHour, endMin, 0, 0);

        // Convertimos a UTC para el backend
        return { StartDate: fromLocalToUTC(startLocal), EndDate: fromLocalToUTC(endLocal) };
      } else {
        // Rango de días: usamos datetime-local de cada slot
        return { StartDate: fromLocalToUTC(slot.startDate), EndDate: fromLocalToUTC(slot.endDate) };
      }
    });
  }

}
