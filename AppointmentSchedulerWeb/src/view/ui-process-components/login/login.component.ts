import { Component, NgZone, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { AuthenticationService } from '../../../cross-cutting/security/authentication/authentication.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { finalize, of, switchMap, take } from 'rxjs';
import { NavigationEnd, Router } from '@angular/router';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { ApiDataErrorResponse, isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { TaskStateManagerService } from '../../model/task-state-manager.service';
import { LoadingState } from '../../model/loading-state.type';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-login',
  imports: [FormsModule, CommonModule, ...SHARED_STANDALONE_COMPONENTS],
  providers: [TaskStateManagerService],
  standalone: true,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})

export class LoginComponent implements OnInit {
  translationCodes = TranslationCodes;
  account: string = '';
  password: string = '';
  systemMessage?: string = '';
  errorValidationMessage: { [field: string]: string[] } = {};

  currentTaskState: LoadingState;


  constructor(private titleService: Title, private authenticationService: AuthenticationService, private router: Router, private ngZone: NgZone, private i18nService: I18nService, private logginService: LoggingService, private stateManagerService: TaskStateManagerService) {
    this.currentTaskState = this.stateManagerService.getState();
    this.stateManagerService.getStateAsObservable().subscribe(state => { this.currentTaskState = state });
  }

  private updateTitle(): void {
    const titleKey = this.translationCodes.TC_LOGIN_DIRECTORY;
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
    this.systemMessage = "";
    this.errorValidationMessage = {};

    if (this.currentTaskState === LoadingState.LOADING) {
      return;
    }

    this.stateManagerService.setState(LoadingState.LOADING);
    this.systemMessage = "";
    this.errorValidationMessage = {};


    this.authenticationService.loginJwt(this.account, this.password).pipe(
      switchMap((response) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return this.authenticationService.getAccountDataFromServer();
        } else {
          this.handleErrorResponse(response);
          return of(response.error);
        }
      }),
      switchMap((isDataReceived) => {
        if (isDataReceived) {
          return this.authenticationService.getAccountData().pipe(
            take(1),
          );
        }
        return of(null);
      })
    ).subscribe({
      next: (accountData) => {
        if (accountData) {
          this.setSuccessfulTask();
        } else {
          this.setUnsuccessfulTask(LoadingState.UNSUCCESSFUL_TASK);
        }
      },
      error: (err) => {
        this.logginService.error(err);
      }
    });
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  signUp() {
    this.router.navigate([WebRoutes.signup])
  }

  private handleErrorResponse(response: OperationResult<boolean, ApiDataErrorResponse>): void {

    let code = getStringEnumKeyByValue(MessageCodeType, MessageCodeType.UNKNOWN_ERROR);
    if (isGenericErrorResponse(response.error)) {
      let codeMessage = getStringEnumKeyByValue(MessageCodeType, response.error.message);
      if (response.error.additionalData?.["field"] !== undefined) {
        this.setErrorValidationMessage(response.error.additionalData["field"], [codeMessage!]);
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
    }, 1000);
  }

  private setSuccessfulTask(): void {
    this.stateManagerService.setState(LoadingState.SUCCESSFUL_TASK);
    setTimeout(() => {
      this.ngZone.run(() => {
        this.router.navigate([WebRoutes.root]);
      });
    }, 1000)
  }


}
