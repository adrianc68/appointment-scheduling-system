import { Component } from '@angular/core';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { Router } from '@angular/router';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AccountService } from '../../../model/communication-components/account.service';
import { finalize, of, switchMap } from 'rxjs';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';

@Component({
  selector: 'app-register',
  imports: [FormsModule, CommonModule, ...SHARED_STANDALONE_COMPONENTS],
  standalone: true,
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {

  translationCodes = TranslationCodes;


  phoneNumber: string = '';
  email: string = '';
  name: string = '';
  username: string = '';
  password: string = '';


  errorValidationMessage: { [field: string]: string[] } = {};
  systemMessage?: string = '';
  isLoading: boolean = false;
  dataLoaded: boolean = false;

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  constructor(private router: Router, private i18nService: I18nService, private loggingService: LoggingService, private accountService: AccountService) { }
  onSubmit() {
    if (!this.isLoading) {
      this.isLoading = true;
      this.dataLoaded = false;
      this.systemMessage = "";
      this.errorValidationMessage = {};
    }


    this.accountService.registerClient(this.username, this.email, this.phoneNumber, this.name, this.password).pipe(
      switchMap((response) => {
        console.log(response);
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(response.result);
        } else {
          let code = getStringEnumKeyByValue(MessageCodeType, MessageCodeType.UNKNOWN_ERROR);
          if (isGenericErrorResponse(response.error)) {
            console.log("genericError");
            let codeMesasge = getStringEnumKeyByValue(MessageCodeType, response.error.message);
            if (response.error.additionalData?.["field"] !== undefined) {
              this.setErrorValidationMessage(response.error.additionalData["field"], [codeMesasge!]);
            }
            code = this.translationCodes.TC_GENERIC_ERROR_CONFLICT;
          } else if (isValidationErrorResponse(response.error)) {

            console.log("ValidationError");

            response.error.forEach(errorItem => {
              this.setErrorValidationMessage(errorItem.field, errorItem.messages);
            });
            code = this.translationCodes.TC_VALIDATION_ERROR;
          } else if (isServerErrorResponse(response.error)) {

            console.log("SErverError");
            code = getStringEnumKeyByValue(MessageCodeType, response.code);
          } else if (isEmptyErrorResponse(response.error)) {

            console.log("EmptyError");
            code = getStringEnumKeyByValue(MessageCodeType, response.code);
          } else {
            console.log('<<<<');
            let code = getStringEnumKeyByValue(MessageCodeType, response.code);
            this.systemMessage = code;
          }

          this.systemMessage = code;
          return of("");
        }
      }),
      finalize(() => {
        this.isLoading = false;
      })
    ).subscribe({
      next: (result) => {
        if (result)
          console.log(result);
      },
      error: (err) => {
        console.log(err);
      }

    });
  }


  private setErrorValidationMessage(key: string, value: string[]) {
    this.errorValidationMessage[key.toLowerCase()] = value;
  }

}
