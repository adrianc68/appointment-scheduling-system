import { Component, OnInit } from '@angular/core';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { NavigationEnd, Router } from '@angular/router';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AccountService } from '../../../model/communication-components/account.service';
import { Observable, of, switchMap } from 'rxjs';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { ApiDataErrorResponse, isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';
import { TaskStateManagerService } from '../../model/task-state-manager.service';
import { LoadingState } from '../../model/loading-state.type';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-register',
  imports: [FormsModule, CommonModule, ...SHARED_STANDALONE_COMPONENTS],
  providers: [TaskStateManagerService],
  standalone: true,
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnInit {

  translationCodes = TranslationCodes;


  phoneNumber: string = '';
  email: string = '';
  name: string = '';
  username: string = '';
  password: string = '';


  errorValidationMessage: { [field: string]: string[] } = {};
  systemMessage?: string = '';
  currentTaskState: LoadingState;

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  constructor(private titleService: Title, private router: Router, private i18nService: I18nService, private loggingService: LoggingService, private accountService: AccountService, private stateManagerService: TaskStateManagerService) {
    this.currentTaskState = this.stateManagerService.getState();
    this.stateManagerService.getStateAsObservable().subscribe(state => { this.currentTaskState = state });
  }


  private updateTitle(): void {
    const titleKey = this.translationCodes.TC_SIGN_UP_DIRECTORY;
    const title = this.i18nService.translate(titleKey);
    this.titleService.setTitle(title);
  }

  ngOnInit(): void {

    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.updateTitle();
      }
    });

    this.updateTitle();
  }

  onSubmit() {
    if (this.currentTaskState === LoadingState.LOADING) {
      return;
    }

    this.stateManagerService.setState(LoadingState.LOADING);
    this.systemMessage = "";
    this.errorValidationMessage = {};

    this.accountService.registerClient(this.username, this.email, this.phoneNumber, this.name, this.password).pipe(
      switchMap((response: OperationResult<string, ApiDataErrorResponse>): Observable<boolean> => {
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
      this.router.navigate([WebRoutes.login])
    }, 1500)
  }

}
