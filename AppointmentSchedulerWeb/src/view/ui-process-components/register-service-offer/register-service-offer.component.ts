import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { AssistantService } from '../../../model/communication-components/assistant.service';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { catchError, map, Observable, of, switchMap } from 'rxjs';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { Assistant } from '../../../view-model/business-entities/assistant';
import { MatIconModule } from '@angular/material/icon';
import { ServiceOffer } from '../../../view-model/business-entities/service-offer';
import { SchedulerService } from '../../../model/communication-components/scheduler.service';
import { Service } from '../../../view-model/business-entities/service';
import { ServiceService } from '../../../model/communication-components/service.service';
import { ServiceOfferStatusType } from '../../../view-model/business-entities/types/service-offer-status.types';
import { ServiceStatusType } from '../../../view-model/business-entities/types/service-status.types';
import { ErrorUIService } from '../../../cross-cutting/communication/handle-error-service/error-ui.service';

@Component({
  selector: 'app-register-service-offer',
  imports: [FormsModule, CommonModule, MatIconModule],
  templateUrl: './register-service-offer.component.html',
  styleUrl: './register-service-offer.component.scss'
})
export class RegisterServiceOfferComponent {
  assistantUuid: string = '';
  newServiceUuid: string = '';
  selectedServices: string[] = [];
  systemMessage: string = '';
  selectedAssistant?: Assistant;
  assistants: Assistant[] = [];
  assistantSelected: string | undefined;
  allServices: Service[] = [];
  servicesOfferAssigned: ServiceOffer[] = [];
  serviceNotAssignedOfAssistant: Service[] = [];
  translationCodes = TranslationCodes;
  currentStep = 1;
  private previousStep = 1;

  constructor(private i18nService: I18nService, private loggingService: LoggingService, private assistantService: AssistantService, private schedulerService: SchedulerService, private serviceService: ServiceService, private errorUIService: ErrorUIService) {
    this.getAssistantList();
    this.getAllServices();
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

    this.systemMessage = 'Servicios asignados correctamente.';
    this.assignServiceToAssistant(payload);
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }



  ngAfterViewChecked(): void {
    if (this.previousStep !== this.currentStep) {
      window.scrollTo(0, 0);
      this.previousStep = this.currentStep;
    }
  }

  nextStep() {
    this.currentStep++;
  }

  prevStep() {
    this.currentStep--;
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

  getAssistantList(): void {
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
        this.loggingService.error(err);
        this.errorUIService.handleError(err);
        return of([]);
      })
    ).subscribe((assistants: Assistant[]) => {
      this.assistants = assistants;
    })
  }

  getAssignedServicesOfAssistant(uuid: string) {
    this.assistantSelected = uuid;
    this.assistantService.getAssignedServiceOfAssistant(uuid).pipe(
      map((response: OperationResult<ServiceOffer[], ApiDataErrorResponse>): boolean => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.servicesOfferAssigned = [...response.result!];
          this.systemMessage = code || "";
          const assignedServiceIds = this.servicesOfferAssigned.map(s => s.serviceUuid);
          this.serviceNotAssignedOfAssistant = this.allServices.filter(
            s => !assignedServiceIds.includes(s.uuid)
          );
          return true;
        } else {
          let code = this.errorUIService.handleError(response);
          this.systemMessage = code || "";
          return false;
        }
      }),
      catchError(err => {
        this.loggingService.error(err);
        this.errorUIService.handleError(err);
        return of(false);
      })
    ).subscribe();
  }

  getAllServices(): void {
    this.serviceService.getServiceList().pipe(
      map((response: OperationResult<Service[], ApiDataErrorResponse>): boolean => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          this.allServices = response.result!.filter(service => service.status === ServiceStatusType.ENABLED);
          return true;
        }
        let code = this.errorUIService.handleError(response);
        this.systemMessage = code || "";
        return false;
      }),
      catchError(err => {
        this.loggingService.error(err);
        this.errorUIService.handleError(err);
        return of(false);
      })
    ).subscribe();
  }



  assignServiceToAssistant(payload: any) {
    this.assistantService.assignServiceToAssistant(payload).pipe(
      map((response: OperationResult<boolean, ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          return true;
        } else {
          return false;
        }
      }),
      catchError(err => {
        this.loggingService.error(err);
        let code = this.errorUIService.handleError(err);
        this.systemMessage = code || "";
        return of([]);
      })
    ).subscribe();
  }

  assignService(uuid: string) {
    const payload = {
      assistantUuid: this.assistantSelected,
      selectedServices: [uuid]
    }

    this.assistantService.addServiceToAssistant(payload).pipe(
      switchMap((response: OperationResult<string[], ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK && response.result?.length) {
          const serviceOfferUuid = response.result[0];

          const index = this.serviceNotAssignedOfAssistant.findIndex(s => s.uuid === uuid);
          if (index !== -1) {
            const service = this.serviceNotAssignedOfAssistant.splice(index, 1)[0];

            const assignedService: ServiceOffer = {
              ...service,
              serviceUuid: service.uuid,
              serviceStatus: service.status,
              uuid: serviceOfferUuid,
              status: ServiceOfferStatusType.ENABLED,
              //assistant: { name: this.assistantSelectedName, uuid: this.assistantSelected }
            };
            this.servicesOfferAssigned.push(assignedService);
          }
          return of(true);
        } else {
          let code = this.errorUIService.handleError(response);

          if (response.code === MessageCodeType.SERVICE_UNAVAILABLE) {
            const index = this.serviceNotAssignedOfAssistant.findIndex(s => s.uuid === uuid);
            if (index !== -1) {
              this.serviceNotAssignedOfAssistant.splice(index, 1)[0];
            }
          }
          this.systemMessage = code || "";
          return of(false);
        }
      }),
    ).subscribe();
  }



  disableAssignedService(uuid: string) {
    this.schedulerService.disableAssignedService(uuid).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          const service = this.servicesOfferAssigned.find(s => s.uuid === uuid);
          if (service) {
            service.status = ServiceOfferStatusType.DISABLED;
          }
          return of(true);
        } else {
          let code = this.errorUIService.handleError(response);
          this.systemMessage = code || "";
          return of(false);
        }
      }),
    ).subscribe();
  }


  enableAssignedService(uuid: string) {
    this.schedulerService.enabledAssignedService(uuid).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          const service = this.servicesOfferAssigned.find(s => s.uuid === uuid);
          if (service) {
            service.status = ServiceOfferStatusType.ENABLED;
          }
          return of(true);
        } else {
          let code = this.errorUIService.handleError(response);
          this.systemMessage = code || "";
          return of(false);
        }
      }),
    ).subscribe();
  }

  deleteAssignedService(uuid: string) {
    this.schedulerService.deleteAssignedService(uuid).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          const index = this.servicesOfferAssigned.findIndex(s => s.uuid === uuid);
          if (index !== -1) {
            const serviceOffer = this.servicesOfferAssigned[index];

            const service: Service = {
              name: serviceOffer.name,
              price: serviceOffer.price,
              minutes: serviceOffer.minutes,
              description: serviceOffer.description,
              uuid: serviceOffer.serviceUuid!,
              status: serviceOffer.serviceStatus ?? ServiceStatusType.ENABLED,
              createdAt: new Date()
            };

            this.serviceNotAssignedOfAssistant.push(service);
            this.servicesOfferAssigned.splice(index, 1);
          }
          return of(true);
        } else {
          let code = this.errorUIService.handleError(response);
          this.systemMessage = code || "";
          return of(false);
        }
      }),
    ).subscribe();
  }






}
