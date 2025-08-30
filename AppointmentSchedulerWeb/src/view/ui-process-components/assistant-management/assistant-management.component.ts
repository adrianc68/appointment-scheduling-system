import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { Assistant } from '../../../view-model/business-entities/assistant';
import { AssistantService } from '../../../model/communication-components/assistant.service';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse, isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { Observable, of, switchMap } from 'rxjs';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';
import { Router } from '@angular/router';
import { TranslatePipe } from '../../../cross-cutting/helper/i18n/translate.pipe';
import { AccountStatusType } from '../../../view-model/business-entities/types/account-status.types';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-assistant-management',
  imports: [CommonModule, ...SHARED_STANDALONE_COMPONENTS, TranslatePipe, MatIconModule],
  standalone: true,
  templateUrl: './assistant-management.component.html',
  styleUrl: './assistant-management.component.scss'
})
export class AssistantManagementComponent {
  systemMessage?: string = '';
  translationCodes = TranslationCodes;
  assistants: Assistant[] = [];

  constructor(private router: Router, private assistantService: AssistantService, private i18nService: I18nService, private logginService: LoggingService) {
    this.assistantService.getAssistantList().pipe(
      switchMap((response: OperationResult<Assistant[], ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.assistants = [...response.result!];
          this.assistants.map(d => console.log(d));
          this.systemMessage = code;
          return of(true);
        } else {
          this.handleErrorResponse(response);
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
        this.logginService.error(err);

      }
    })
  }


  private handleErrorResponse(response: OperationResult<Assistant[], ApiDataErrorResponse>): void {

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


  get enabledClientsCount(): number {
    return this.assistants.filter(assistant => assistant.status === AccountStatusType.ENABLED).length;
  }

  get disabledClientsCount(): number {
    return this.assistants.filter(assistant => assistant.status === AccountStatusType.DISABLED).length;
  }

  redirectToRegisterAssistant() {
    this.router.navigate([WebRoutes.assistant_management_register_assistant])
  }

  redirectToEditAssistant(assistant: Assistant) {
    this.router.navigate([WebRoutes.assistant_management_edit_assistant], { state: { assistant } });
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }


}
