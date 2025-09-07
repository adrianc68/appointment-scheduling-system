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
import { catchError, map, Observable, of, switchMap } from 'rxjs';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { Router } from '@angular/router';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';
import { MatIconModule } from '@angular/material/icon';
import { ErrorUIService } from '../../../cross-cutting/communication/handle-error-service/error-ui.service';

@Component({
  selector: 'app-service-offer-management',
  imports: [CommonModule, TranslatePipe, ...SHARED_STANDALONE_COMPONENTS, MatIconModule],
  standalone: true,
  templateUrl: './service-offer-management.component.html',
  styleUrl: './service-offer-management.component.scss'
})
export class ServiceOfferManagementComponent {
  TranslationCodes = TranslationCodes;
  serviceOffers: ServiceAssignment[] = [];

  constructor(private assistantService: AssistantService, private logginService: LoggingService, private router: Router, private errorUIService: ErrorUIService) {
    this.getServiceOfferList();

  }

  private getServiceOfferList(): void {
    this.assistantService.getAssistantsAndServicesOffersList().pipe(
      map((response: OperationResult<ServiceAssignment[], ApiDataErrorResponse>) => {
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
    ).subscribe((servicesAssigment: ServiceAssignment[]) => {
      this.serviceOffers = servicesAssigment;
    })
  }

  redirectToRegisterServiceOffer(): void {
    this.router.navigate([WebRoutes.service_offer_management_register_service_offer])
  }

  openedSlots = new Set<number>();

  toggleSlot(index: number) {
    if (this.openedSlots.has(index)) {
      this.openedSlots.delete(index);
    } else {
      this.openedSlots.add(index);
    }
  }






}

