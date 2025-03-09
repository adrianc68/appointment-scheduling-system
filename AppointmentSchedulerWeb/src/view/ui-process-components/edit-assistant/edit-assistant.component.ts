import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { TaskStateManagerService } from '../../model/task-state-manager.service';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { LoadingState } from '../../model/loading-state.type';
import { Assistant } from '../../../view-model/business-entities/assistant';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { AccountService } from '../../../model/communication-components/account.service';
import { AssistantService } from '../../../model/communication-components/assistant.service';
import { Observable, of, switchMap } from 'rxjs';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse, isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';

@Component({
  selector: 'app-edit-assistant',
  imports: [FormsModule, CommonModule, ...SHARED_STANDALONE_COMPONENTS],
  standalone: true,
  providers: [TaskStateManagerService],
  templateUrl: './edit-assistant.component.html',
  styleUrl: './edit-assistant.component.scss'
})
export class EditAssistantComponent {
  translationCodes = TranslationCodes;
  errorValidationMessage: { [field: string]: string[] } = {};
  systemMessage?: string = '';
  currentTaskState: LoadingState;

  assistant: Assistant;

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  constructor(private titleService: Title, private router: Router, private i18nService: I18nService, private loggingService: LoggingService, private accountService: AccountService, private assistantService: AssistantService, private stateManagerService: TaskStateManagerService) {
    this.assistant = this.router.getCurrentNavigation()?.extras.state?.["assistant"];
    this.currentTaskState = this.stateManagerService.getState();
    this.stateManagerService.getStateAsObservable().subscribe(state => { this.currentTaskState = state });
  }

  onSubmit() {
    console.log("called ")
    if (this.currentTaskState === LoadingState.LOADING) {
      return;
    }

    this.stateManagerService.setState(LoadingState.LOADING);
    this.systemMessage = "";
    this.errorValidationMessage = {};


    this.assistantService.editAssistant(this.assistant).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        console.log("editclient called")
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          return of(true);
        } else {
          this.handleErrorResponse(response);
          return of(false);
        }
      }),
    ).subscribe({
      next: (result) => {
        if (result) {
          console.log("<<<<");
          this.setSuccessfulTask();
        } else {
          console.log("<<<<");
          this.setUnsuccessfulTask(LoadingState.UNSUCCESSFUL_TASK);
        }
      },
      error: (err) => {
        this.loggingService.error(err);
        this.setUnsuccessfulTask(LoadingState.UNSUCCESSFUL_TASK);
      }
    });
  }

  disableAssistant(uuid: string) {
    this.assistantService.disableAssistant(uuid).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(true);
        } else {
          this.handleErrorResponse(response);
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

  enableAssistant(uuid: string) {
    this.assistantService.enableAssistant(uuid).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(true);
        } else {
          this.handleErrorResponse(response);
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

  deleteAssistant(uuid: string) {
    this.assistantService.deleteAssistant(uuid).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(true);
        } else {
          this.handleErrorResponse(response);
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


  private handleErrorResponse(response: OperationResult<boolean, ApiDataErrorResponse>): void {

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
      //
    }, 1500)
  }

}
