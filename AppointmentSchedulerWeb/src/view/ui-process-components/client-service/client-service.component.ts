import { Component } from '@angular/core';
import { ServiceService } from '../../../model/communication-components/service.service';
import { catchError, map, of } from 'rxjs';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { Service } from '../../../view-model/business-entities/service';
import { ApiDataErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { ErrorUIService } from '../../../cross-cutting/communication/handle-error-service/error-ui.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { CommonModule } from '@angular/common';
import { SchedulerService } from '../../../model/communication-components/scheduler.service';

@Component({
  selector: 'app-client-service',
  imports: [CommonModule],
  templateUrl: './client-service.component.html',
  styleUrl: './client-service.component.scss'
})
export class ClientServiceComponent {
  services: Service[] = [];


  constructor(private serviceService: ServiceService, private schedulerService: SchedulerService, private errorUIService: ErrorUIService, private loggingService: LoggingService) {
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


}
