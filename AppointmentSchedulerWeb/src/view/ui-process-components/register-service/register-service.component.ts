import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { TaskStateManagerService } from '../../model/task-state-manager.service';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { LoadingState } from '../../model/loading-state.type';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { ServiceService } from '../../../model/communication-components/service.service';
import { catchError, map, Observable, of, switchMap } from 'rxjs';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse, isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { ErrorUIService } from '../../../cross-cutting/communication/handle-error-service/error-ui.service';

@Component({
  selector: 'app-register-service',
  imports: [FormsModule, CommonModule, ...SHARED_STANDALONE_COMPONENTS],
  providers: [TaskStateManagerService],
  standalone: true,
  templateUrl: './register-service.component.html',
  styleUrl: './register-service.component.scss'
})
export class RegisterServiceComponent {
  translationCodes = TranslationCodes;

  name: string = "";
  description: string = "";
  minutes: number = 0;
  price: number = 0;

  errorValidationMessage: { [field: string]: string[] } = {};
  systemMessage?: string = '';
  loadingState: LoadingState = LoadingState.NO_ACTION_PERFORMED;

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  constructor(private titleService: Title, private router: Router, private i18nService: I18nService, private loggingService: LoggingService, private serviceService: ServiceService, private errorUIService: ErrorUIService) {
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


    this.registerService().subscribe(result => {
      if (result) {
        this.loadingState = LoadingState.SUCCESSFUL_TASK;
      } else {
        this.loadingState = LoadingState.UNSUCCESSFUL_TASK;
      }
    })
  }

  private registerService(): Observable<boolean> {
    return this.serviceService.registerService(this.description, this.name, this.minutes, this.price).pipe(
      map((response: OperationResult<string, ApiDataErrorResponse>) => {
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



  private setErrorValidationMessage(key: string, value: string[]) {
    this.errorValidationMessage[key.toLowerCase()] = value;
  }

}
