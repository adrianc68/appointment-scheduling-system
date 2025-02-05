import { Component } from '@angular/core';
import { ClientService } from '../../../model/communication-components/client.service';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { ApiDataErrorResponse, isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { ApiResponse } from '../../../cross-cutting/communication/model/api-response';
import { Client } from '../../../view-model/business-entities/client';
import { map, Observable, of, switchMap } from 'rxjs';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { CommonModule } from '@angular/common';
import { ɵInternalFormsSharedModule } from '@angular/forms';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';

@Component({
  selector: 'app-client-management',
  imports: [CommonModule, ...SHARED_STANDALONE_COMPONENTS],
  standalone: true,
  templateUrl: './client-management.component.html',
  styleUrl: './client-management.component.scss'
})
export class ClientManagementComponent {
  systemMessage?: string = '';
  translationCodes = TranslationCodes;
  clients: Client[] = [];

  constructor(private clientService: ClientService, private i18nService: I18nService, private logginService: LoggingService) {
    this.clientService.getClientList().pipe(
      switchMap((response: OperationResult<Client[], ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.clients = [...response.result!];
          this.clients.map(d => console.log(d));
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


  private handleErrorResponse(response: OperationResult<Client[], ApiDataErrorResponse>): void {

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
