import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { Assistant } from '../../../view-model/business-entities/assistant';
import { AssistantService } from '../../../model/communication-components/assistant.service';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { catchError, map, of } from 'rxjs';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';
import { Router } from '@angular/router';
import { TranslatePipe } from '../../../cross-cutting/helper/i18n/translate.pipe';
import { AccountStatusType } from '../../../view-model/business-entities/types/account-status.types';
import { MatIconModule } from '@angular/material/icon';
import { ErrorUIService } from '../../../cross-cutting/communication/handle-error-service/error-ui.service';

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

  constructor(private router: Router, private assistantService: AssistantService, private i18nService: I18nService, private logginService: LoggingService, private errorUIService: ErrorUIService) {
    this.getAssistantList();
  }

  private getAssistantList(): void {
    this.assistantService.getAssistantList().pipe(
      map((response: OperationResult<Assistant[], ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK && response.result) {
          return response.result;
        } else {
          this.errorUIService.handleError(response);
          return [];
        }
      }),
      catchError(err => {
        this.logginService.error(err);
        this.errorUIService.handleError(err);
        return of([]);
      })
    ).subscribe((assistants: Assistant[]) => {
      this.assistants = assistants;
    })
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
