import { Component } from '@angular/core';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { CommonModule } from '@angular/common';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { Service } from '../../../view-model/business-entities/service';
import { catchError, map, of } from 'rxjs';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';
import { Router } from '@angular/router';
import { ServiceService } from '../../../model/communication-components/service.service';
import { ServiceGridItemComponent } from '../../ui-components/display/grid-list/service-grid-item/service-grid-item.component';
import { TranslatePipe } from '../../../cross-cutting/helper/i18n/translate.pipe';
import { ServiceStatusType } from '../../../view-model/business-entities/types/service-status.types';
import { MatIconModule } from '@angular/material/icon';
import { ErrorUIService } from '../../../cross-cutting/communication/handle-error-service/error-ui.service';

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

  constructor(private serviceService: ServiceService, private i18nService: I18nService, private loggingService: LoggingService, private router: Router, private errorUIService: ErrorUIService) {
    this.getServiceList();
  }

  private getServiceList(): void {
    this.serviceService.getServiceList().pipe(
      map((response: OperationResult<Service[], ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK && response.result) {
          return response.result;
        } else {
          this.errorUIService.handleError(response);
          return [];
        }
      }),
      catchError(err => {
        this.loggingService.error(err);
        this.errorUIService.handleError(err);
        return of([]);
      })
    ).subscribe((services: Service[]) => {
      this.services = services;
    });
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
