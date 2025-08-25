import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { AssistantService } from '../../../model/communication-components/assistant.service';
import { LoadingState } from '../../model/loading-state.type';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse, isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { Observable, of, switchMap } from 'rxjs';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';

@Component({
  selector: 'app-register-service-offer',
  imports: [FormsModule, CommonModule],
  templateUrl: './register-service-offer.component.html',
  styleUrl: './register-service-offer.component.scss'
})
export class RegisterServiceOfferComponent {
  assistantUuid: string = '';
  newServiceUuid: string = '';
  selectedServices: string[] = [];
  systemMessage: string | undefined = '';

  constructor(private titleService: Title, private router: Router, private i18nService: I18nService, private loggingService: LoggingService, private assistantService: AssistantService) {
  }

  addService() {
    if (this.newServiceUuid.trim()) {
      this.selectedServices.push(this.newServiceUuid.trim());
      this.newServiceUuid = '';
    }
  }

  removeService(index: number) {
    this.selectedServices.splice(index, 1);
  }

  onSubmit() {
    if (!this.assistantUuid || this.selectedServices.length === 0) {
      this.systemMessage = 'Debes ingresar un Assistant UUID y al menos un servicio.';
      return;
    }

    const payload = {
      assistantUuid: this.assistantUuid,
      selectedServices: this.selectedServices
    };

    console.log('Enviando payload:', payload);
    this.systemMessage = 'Servicios asignados correctamente.';


    this.assistantService.assignServiceToAssistant(payload).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          return of(true);
        } else {
          this.handleErrorResponse(response);
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(false);
        }
      }),
    ).subscribe({
      next: (result) => {
        if (result) {
          //this.setSuccessfulTask();
        } else {
          //this.setUnsuccessfulTask(LoadingState.UNSUCCESSFUL_TASK);
        }
      },
      error: (err) => {
        this.loggingService.error(err);
        //this.setUnsuccessfulTask(LoadingState.UNSUCCESSFUL_TASK);
      }
    });

  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }



  translationCodes = TranslationCodes;


  private handleErrorResponse(response: OperationResult<boolean, ApiDataErrorResponse>): void {

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


}
