import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { AccountService } from '../../../model/communication-components/account.service';
import { TaskStateManagerService } from '../../model/task-state-manager.service';
import { LoadingState } from '../../model/loading-state.type';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { catchError, map, Observable, of } from 'rxjs';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { FormsModule } from '@angular/forms';
import { ErrorUIService } from '../../../cross-cutting/communication/handle-error-service/error-ui.service';

@Component({
  selector: 'app-register-client',
  imports: [FormsModule, CommonModule, ...SHARED_STANDALONE_COMPONENTS],
  standalone: true,
  providers: [TaskStateManagerService],
  templateUrl: './register-client.component.html',
  styleUrl: './register-client.component.scss'
})
export class RegisterClientComponent {
  translationCodes = TranslationCodes;

  phoneNumber: string = '';
  email: string = '';
  name: string = '';
  username: string = '';
  password: string = '';

  errorValidationMessage: { [field: string]: string[] } = {};

  loadingState: LoadingState = LoadingState.NO_ACTION_PERFORMED;
  systemMessage?: string = '';

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  constructor(private titleService: Title, private router: Router, private i18nService: I18nService, private loggingService: LoggingService, private accountService: AccountService, private errorUIService: ErrorUIService) {
  }

  onSubmit() {
    if (this.loadingState === LoadingState.LOADING || this.loadingState == LoadingState.WORK_DONE) {
      return;
    }

    if (this.loadingState === LoadingState.SUCCESSFUL_TASK) {
      this.loadingState = LoadingState.WORK_DONE;
      return;
    }


    this.loadingState = LoadingState.LOADING;
    this.systemMessage = "";
    this.errorValidationMessage = {};


    this.registerClient(this.username, this.email, this.phoneNumber, this.name, this.password).subscribe(result => {
      if (result) {
        this.loadingState = LoadingState.SUCCESSFUL_TASK;
      } else {
        this.loadingState = LoadingState.UNSUCCESSFUL_TASK;
      }
    });
  }



  private registerClient(username: string, email: string, phoneNumber: string, name: string, password: string): Observable<boolean> {
    return this.accountService.registerClient(username, email, phoneNumber, name, password).pipe(
      map((response: OperationResult<string, ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
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
