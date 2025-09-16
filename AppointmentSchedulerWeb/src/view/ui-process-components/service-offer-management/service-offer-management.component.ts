import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { TranslatePipe } from '../../../cross-cutting/helper/i18n/translate.pipe';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { AssistantService } from '../../../model/communication-components/assistant.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { ServiceAssignment } from '../../../view-model/business-entities/service-assignment';
import { catchError, map, of } from 'rxjs';
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

  getTotalServices(): number {
    return this.serviceOffers.reduce((acc, sa) => acc + sa.serviceOffer.length, 0);
  }

  getAverageServices(): string {
    if (this.serviceOffers.length === 0) return "0.00";
    return (this.getTotalServices() / this.serviceOffers.length).toFixed(2);
  }







}

