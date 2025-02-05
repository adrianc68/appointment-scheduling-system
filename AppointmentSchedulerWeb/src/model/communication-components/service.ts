import { Injectable } from "@angular/core";
import { HttpClientAdapter } from "../../cross-cutting/communication/http-client-adapter-service/http-client-adapter.service";
import { ConfigService } from "../../cross-cutting/operation-management/configService/config.service";
import { ApiRoutes, ApiVersionRoute } from "../../cross-cutting/operation-management/model/api-routes.constants";
import { catchError, map, Observable, of, throwError } from "rxjs";
import { OperationResult } from "../../cross-cutting/communication/model/operation-result.response";
import { Service } from "../../view-model/business-entities/service";
import { ApiDataErrorResponse } from "../../cross-cutting/communication/model/api-response.error";
import { ServiceDTO } from "../dtos/service.dto";
import { OperationResultService } from "../../cross-cutting/communication/model/operation-result.service";
import { ApiResponse } from "../../cross-cutting/communication/model/api-response";
import { HttpErrorResponse } from "@angular/common/http";
import { MessageCodeType } from "../../cross-cutting/communication/model/message-code.types";

@Injectable({
  providedIn: 'root'
})

export class ServiceService {
  private apiUrl: string;

  constructor(private httpServiceAdapter: HttpClientAdapter, private configService: ConfigService) {
    this.apiUrl = this.configService.getApiBaseUrl() + ApiVersionRoute.v1;
  }

  getServiceList(): Observable<OperationResult<Service[], ApiDataErrorResponse>> {
    return this.httpServiceAdapter.get<ServiceDTO[]>(`${this.apiUrl}${ApiRoutes.getAllServices}`).pipe(
      map((response: ApiResponse<ServiceDTO[], ApiDataErrorResponse>) => {
        console.log(response);
        if (this.httpServiceAdapter.isSuccessResponse<ServiceDTO[]>(response)) {
          const services: Service[] = response.data.map(dto => this.parseService(dto));
          return OperationResultService.createSuccess(services, response.message);
        }
        return OperationResultService.createFailure(response.data, response.message);
      }),
      catchError((err) => {
        if (err instanceof HttpErrorResponse) {
          let codeError = MessageCodeType.UNKNOWN_ERROR;
          if (err.status == 500) {
            codeError = MessageCodeType.SERVER_ERROR;
          }
          return of(OperationResultService.createFailure<ApiDataErrorResponse>(err.error, codeError));
        }
        return throwError(() => err);
      })
    );
  }

  private parseService(dto: ServiceDTO): Service {
    let data = new Service(dto.description, dto.minutes, dto.name, dto.price, dto.uuid, dto.status, dto.createdAt);
    return data;
  }

}
