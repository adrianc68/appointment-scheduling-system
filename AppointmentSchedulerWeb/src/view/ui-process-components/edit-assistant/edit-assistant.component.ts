import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { TaskStateManagerService } from '../../model/task-state-manager.service';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { LoadingState } from '../../model/loading-state.type';
import { Assistant } from '../../../view-model/business-entities/assistant';
import { Router } from '@angular/router';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { AssistantService } from '../../../model/communication-components/assistant.service';
import { catchError, map, Observable, of, switchMap } from 'rxjs';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { MatIconModule } from '@angular/material/icon';
import { ErrorUIService } from '../../../cross-cutting/communication/handle-error-service/error-ui.service';

@Component({
  selector: 'app-edit-assistant',
  imports: [FormsModule, CommonModule, ...SHARED_STANDALONE_COMPONENTS, MatIconModule],
  standalone: true,
  providers: [TaskStateManagerService],
  templateUrl: './edit-assistant.component.html',
  styleUrl: './edit-assistant.component.scss'
})
export class EditAssistantComponent {
  translationCodes = TranslationCodes;
  errorValidationMessage: { [field: string]: string[] } = {};
  systemMessage?: string = '';
  loadingState: LoadingState = LoadingState.NO_ACTION_PERFORMED;

  assistant: Assistant;

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  constructor(private router: Router, private i18nService: I18nService, private loggingService: LoggingService, private assistantService: AssistantService, private errorUIService: ErrorUIService) {
    this.assistant = this.router.getCurrentNavigation()?.extras.state?.["assistant"];
  }

  onSubmit() {
    if (this.loadingState == LoadingState.LOADING || this.loadingState == LoadingState.WORK_DONE) {
      return;
    }

    if (this.loadingState === LoadingState.SUCCESSFUL_TASK) {
      this.loadingState = LoadingState.WORK_DONE;
      return;
    }

    this.systemMessage = "";
    this.errorValidationMessage = {};
    this.loadingState = LoadingState.LOADING;

    this.editAssistant().subscribe(result => {
      if (result) {
        this.loadingState = LoadingState.SUCCESSFUL_TASK;
      } else {
        this.loadingState = LoadingState.UNSUCCESSFUL_TASK;
      }
    });
  }


  editAssistant(): Observable<boolean> {
    return this.assistantService.editAssistant(this.assistant).pipe(
      map((response: OperationResult<boolean, ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
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
        this.loggingService.error(err);
        return of(false);
      })
    );
  }

  disableAssistant(uuid: string) {
    this.assistantService.disableAssistant(uuid).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(true);
        } else {
          this.errorUIService.handleError(response);
          const validationErrors = this.errorUIService.getValidationErrors(response);
          Object.entries(validationErrors).forEach(([field, messages]) => {
            this.setErrorValidationMessage(field, messages);
          });
          return of(false);
        }
      }),
    ).subscribe();
  }

  enableAssistant(uuid: string) {
    this.assistantService.enableAssistant(uuid).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(true);
        } else {
          this.errorUIService.handleError(response);

          return of(false);
        }
      }),
    ).subscribe();
  }

  deleteAssistant(uuid: string) {
    this.assistantService.deleteAssistant(uuid).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(true);
        } else {
          this.errorUIService.handleError(response);
          return of(false);
        }
      }),
    ).subscribe();
  }

  private setErrorValidationMessage(key: string, value: string[]) {
    this.errorValidationMessage[key.toLowerCase()] = value;
  }
}
