import { Component } from '@angular/core';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { CommonModule } from '@angular/common';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { Service } from '../../../view-model/business-entities/service';
import { Observable, of, switchMap } from 'rxjs';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse, isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';
import { Router } from '@angular/router';
import { ServiceService } from '../../../model/communication-components/service.service';
import { ServiceGridItemComponent } from '../../ui-components/display/grid-list/service-grid-item/service-grid-item.component';
import { TranslatePipe } from '../../../cross-cutting/helper/i18n/translate.pipe';
import { ServiceStatusType } from '../../../view-model/business-entities/types/service-status.types';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-service-management',
  imports: [CommonModule, ...SHARED_STANDALONE_COMPONENTS, TranslatePipe, MatIconModule],
  standalone: true,
  templateUrl: './service-management.component.html',
  styleUrl: './service-management.component.scss'
})
export class ServiceManagementComponent {
  systemMessage?: string = '';
  translationCodes = TranslationCodes;
  services: Service[] = [];
  serviceCard = ServiceGridItemComponent;

  constructor(private serviceService: ServiceService, private i18nService: I18nService, private loggingService: LoggingService, private router: Router) {
    this.serviceService.getServiceList().pipe(
      switchMap((response: OperationResult<Service[], ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.services = [...response.result!];
          this.services.map(s => console.log(s));
          this.systemMessage = code;
          return of(true);
        }
        this.handleErrorResponse(response);
        return of(false);

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
        this.loggingService.error(err);

      }
    });

  }


  private handleErrorResponse(response: OperationResult<Service[], ApiDataErrorResponse>): void {

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


  get enabledServiceCount(): number {
    return this.services.filter(item => item.status === ServiceStatusType.ENABLED).length;
  }

  get disabledServiceCount(): number {
    return this.services.filter(item => item.status === ServiceStatusType.DISABLED).length;
  }


  redirectToEditService(service: Service) {
    this.router.navigate([WebRoutes.service_management_edit_service], { state: { service } });
  }

  redirectToRegisterService() {
    this.router.navigate([WebRoutes.service_management_register_service]);
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }








}
