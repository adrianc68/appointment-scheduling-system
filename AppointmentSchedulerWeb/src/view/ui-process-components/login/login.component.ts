import { Component, NgZone, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { AuthenticationService } from '../../../cross-cutting/security/authentication/authentication.service';
import { of, switchMap, take } from 'rxjs';
import { NavigationEnd, Router } from '@angular/router';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { ApiDataErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { TaskStateManagerService } from '../../model/task-state-manager.service';
import { LoadingState } from '../../model/loading-state.type';
import { Title } from '@angular/platform-browser';
import { ErrorUIService } from '../../../cross-cutting/communication/handle-error-service/error-ui.service';

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

  loadingState: LoadingState = LoadingState.NO_ACTION_PERFORMED;


  constructor(private titleService: Title, private authenticationService: AuthenticationService, private router: Router, private i18nService: I18nService, private errorUIService: ErrorUIService) {
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


    this.authenticationService.loginJwt(this.account, this.password).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          return this.authenticationService.getAccountDataFromServer().pipe(
            switchMap((result) => {
              if (result.isSuccessful) {
                this.loadingState = LoadingState.SUCCESSFUL_TASK;
                return this.authenticationService.getAccountData().pipe(take(1));
              }
              return of(null);
            })
          );
        } else {
          this.errorUIService.handleError(response);
          const validationErrors = this.errorUIService.getValidationErrors(response);
          Object.entries(validationErrors).forEach(([field, messages]) => {
            this.setErrorValidationMessage(field, messages);
          });
          this.loadingState = LoadingState.UNSUCCESSFUL_TASK;
          return of(null);
        }
      })
    ).subscribe({
      next: (accountData) => {
        if (accountData) {
          this.router.navigate(['/']);
        }
      }
    });



  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  signUp() {
    this.router.navigate([WebRoutes.signup])
  }

  private setErrorValidationMessage(key: string, value: string[]) {
    this.errorValidationMessage[key.toLowerCase()] = value;
  }


}
