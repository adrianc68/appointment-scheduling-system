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
import { Assistant } from '../../../view-model/business-entities/assistant';
import { MatIconModule } from '@angular/material/icon';
import { ServiceOffer } from '../../../view-model/business-entities/service-offer';
import { SchedulerService } from '../../../model/communication-components/scheduler.service';
import { Service } from '../../../view-model/business-entities/service';
import { ServiceService } from '../../../model/communication-components/service.service';
import { ServiceOfferStatusType } from '../../../view-model/business-entities/types/service-offer-status.types';
import { ServiceStatusType } from '../../../view-model/business-entities/types/service-status.types';

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
  systemMessage: string | undefined = '';

  assistants: Assistant[] = [];
  assistantSelected: string | undefined;

  constructor(private titleService: Title, private router: Router, private i18nService: I18nService, private loggingService: LoggingService, private assistantService: AssistantService, private schedulerService: SchedulerService, private serviceService: ServiceService) {
    this.assistantService.getAssistantList().pipe(
      switchMap((response: OperationResult<Assistant[], ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.assistants = [...response.result!];
          this.assistants.map(d => console.log(d));
          this.systemMessage = code;
          return of(true);
        } else {
          this.handleErrorResponseAssistant(response);
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
        this.loggingService.error(err);

      }
    });


    this.getAllServices();
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




  private handleErrorResponseAssistant(response: OperationResult<Assistant[], ApiDataErrorResponse>): void {

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


  private handleErrorAssignedServiceAssistant(response: OperationResult<ServiceOffer[], ApiDataErrorResponse>): void {

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




  allServices: Service[] = [];
  servicesOfferAssigned: ServiceOffer[] = [];
  serviceNotAssignedOfAssistant: Service[] = [];


  getAssignedServicesOfAssistant(uuid: string) {
    this.assistantSelected = uuid;

    this.assistantService.getAssignedServiceOfAssistant(uuid).pipe(
      switchMap((response: OperationResult<ServiceOffer[], ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.servicesOfferAssigned = [...response.result!];

          this.assistants.map(d => console.log(d));
          this.systemMessage = code;
          //const assignedServiceIds = this.servicesOfferAssigned.map(s => s.uuid);
          //this.serviceNotAssignedOfAssistant = this.allServices.filter(s => !assignedServiceIds.includes(s.uuid));
          const assignedServiceIds = this.servicesOfferAssigned.map(s => s.serviceUuid);
          console.log("SERVICIOS ASIGNADOS");
          console.log(this.servicesOfferAssigned);
          console.log("TODOS LOS SERVICIOS");
          console.log(this.allServices);

          // Filtrar todos los servicios para quedarnos solo con los NO asignados
          this.serviceNotAssignedOfAssistant = this.allServices.filter(
            s => !assignedServiceIds.includes(s.uuid)
          );
          return of(true);
        } else {
          this.handleErrorAssignedServiceAssistant(response);
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
        this.loggingService.error(err);

      }
    });
  }



  getAllServices() {
    this.serviceService.getServiceList().pipe(
      switchMap((response: OperationResult<Service[], ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.allServices = [...response.result!];
          this.allServices.map(s => console.log(s));
          this.systemMessage = code;
          return of(true);
        }
        this.handleErrorResponseServices(response);
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
    })
  }

  private handleErrorResponseServices(response: OperationResult<Service[], ApiDataErrorResponse>): void {

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

  deleteAssignedService(uuid: string) {
    this.schedulerService.deleteAssignedService(uuid).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          const index = this.servicesOfferAssigned.findIndex(s => s.uuid === uuid);
          if (index !== -1) {
            const serviceOffer = this.servicesOfferAssigned[index];


            console.log("ServiceOffer DATA <<<<<<<<<<<<<<<,");
            console.log(serviceOffer);

            const service: Service = {
              name: serviceOffer.name,
              price: serviceOffer.price,
              minutes: serviceOffer.minutes,
              description: serviceOffer.description,
              uuid: serviceOffer.serviceUuid!,
              status: serviceOffer.serviceStatus ?? ServiceStatusType.ENABLED,
              createdAt: new Date()
            };

            console.log("SErvice CREATED FROM ServiceOFFER <<<<<<<<<<")

            console.log(service);

            this.serviceNotAssignedOfAssistant.push(service);

            this.servicesOfferAssigned.splice(index, 1);
          }
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

  private handleErrorResponseServicePatch(response: OperationResult<string[], ApiDataErrorResponse>): void {

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


  assignService(uuid: string) {
    const payload = {
      assistantUuid: this.assistantSelected,
      selectedServices: [
        uuid
      ]

    }

    this.assistantService.addServiceToAssistant(payload).pipe(
      switchMap((response: OperationResult<string[], ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK && response.result?.length) {
          const serviceOfferUuid = response.result[0]; // ahora TypeScript sabe que existe

          const index = this.serviceNotAssignedOfAssistant.findIndex(s => s.uuid === uuid);
          if (index !== -1) {
            const service = this.serviceNotAssignedOfAssistant.splice(index, 1)[0];

            // Convertir a ServiceOffer antes de agregar
            const assignedService: ServiceOffer = {
              ...service,
              serviceUuid: service.uuid,
              serviceStatus: service.status,
              uuid: serviceOfferUuid,
              status: ServiceOfferStatusType.ENABLED,
              //assistant: { name: this.assistantSelectedName, uuid: this.assistantSelected }
            };


            console.log("Service");
            console.log(service);

            console.log("CREATED ServiceOffer from Service DATA");
            console.log(assignedService);

            this.servicesOfferAssigned.push(assignedService);
          }
          return of(true);
        } else {
          this.handleErrorResponseServicePatch(response);
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


}
