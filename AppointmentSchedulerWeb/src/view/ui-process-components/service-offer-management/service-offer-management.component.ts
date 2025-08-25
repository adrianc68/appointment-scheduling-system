import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { TranslatePipe } from '../../../cross-cutting/helper/i18n/translate.pipe';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { AssistantService } from '../../../model/communication-components/assistant.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse, isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { ServiceAssignment } from '../../../view-model/business-entities/service-assignment';
import { Observable, of, switchMap } from 'rxjs';
import { ServiceAssignedGridItemComponent } from '../../ui-components/display/grid-list/service-assigned-grid-item/service-assigned-grid-item.component';
import { GridListComponent } from '../../ui-components/display/grid-list/grid-list.component';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { Router } from '@angular/router';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';

@Component({
  selector: 'app-service-offer-management',
  imports: [CommonModule, TranslatePipe, ...SHARED_STANDALONE_COMPONENTS],
  standalone: true,
  templateUrl: './service-offer-management.component.html',
  styleUrl: './service-offer-management.component.scss'
})
export class ServiceOfferManagementComponent {
  TranslationCodes = TranslationCodes;
  serviceOffers: ServiceAssignment[] = [];
  component = ServiceAssignedGridItemComponent;

  constructor(private assistantService: AssistantService, private logginService: LoggingService, private router: Router) {
    this.assistantService.getAssistantsAndServicesOffersList().pipe(
      switchMap((response: OperationResult<ServiceAssignment[], ApiDataErrorResponse>): Observable<boolean> => {
        console.log(response);
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.serviceOffers = [...response.result!];
          //this.clients.map(d => console.log(d));
          //this.systemMessage = code;
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

  private handleErrorResponse(response: OperationResult<any[], ApiDataErrorResponse>): void {

    let code = getStringEnumKeyByValue(MessageCodeType, MessageCodeType.UNKNOWN_ERROR);
    if (isGenericErrorResponse(response.error)) {
      code = this.TranslationCodes.TC_GENERIC_ERROR_CONFLICT;
    } else if (isValidationErrorResponse(response.error)) {
      code = this.TranslationCodes.TC_VALIDATION_ERROR;
    } else if (isServerErrorResponse(response.error)) {
      code = getStringEnumKeyByValue(MessageCodeType, response.code);
    } else if (isEmptyErrorResponse(response.error)) {
      code = getStringEnumKeyByValue(MessageCodeType, response.code);
    }

    //this.systemMessage = code;
  }


  redirectToRegisterServiceOffer(): void {
    this.router.navigate([WebRoutes.service_offer_management_register_service_offer])
  }



}
