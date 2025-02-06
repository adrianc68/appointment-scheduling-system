import { Injectable } from "@angular/core";
import { HttpClientAdapter } from "../../cross-cutting/communication/http-client-adapter-service/http-client-adapter.service";
import { ConfigService } from "../../cross-cutting/operation-management/configService/config.service";
import { ApiRoutes, ApiVersionRoute } from "../../cross-cutting/operation-management/model/api-routes.constants";
import { OperationResult } from "../../cross-cutting/communication/model/operation-result.response";
import { Client } from "../../view-model/business-entities/client";
import { ApiDataErrorResponse } from "../../cross-cutting/communication/model/api-response.error";
import { catchError, map, Observable, of, throwError } from "rxjs";
import { ApiResponse } from "../../cross-cutting/communication/model/api-response";
import { ClientDTO } from "../dtos/client.dto";
import { HttpErrorResponse } from "@angular/common/http";
import { MessageCodeType } from "../../cross-cutting/communication/model/message-code.types";
import { OperationResultService } from "../../cross-cutting/communication/model/operation-result.service";
import { RoleType } from "../../view-model/business-entities/types/role.types";

@Injectable({
  providedIn: 'root'
})

export class ClientService {
  private apiUrl: string;

  constructor(private httpServiceAdapter: HttpClientAdapter, private configService: ConfigService) {
    this.apiUrl = this.configService.getApiBaseUrl() + ApiVersionRoute.v1;
  }

  getClientList(): Observable<OperationResult<Client[], ApiDataErrorResponse>> {
    return this.httpServiceAdapter.get<ClientDTO[]>(`${this.apiUrl}${ApiRoutes.getAllClients}`).pipe(
      map((response: ApiResponse<ClientDTO[], ApiDataErrorResponse>) => {
        console.log(response);
        if (this.httpServiceAdapter.isSuccessResponse<ClientDTO[]>(response)) {
          const clients: Client[] = response.data.map(dto => this.parseClient(dto));
          return OperationResultService.createSuccess(clients, response.message);
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

  editClient(client: Client): Observable<OperationResult<boolean, ApiDataErrorResponse>> {
    return this.httpServiceAdapter.put<boolean>(`${this.apiUrl}${ApiRoutes.editClient}`, client).pipe(
      map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<boolean>(response)) {
          return OperationResultService.createSuccess(response.data, response.message);
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
    )
  }

  disableClient(uuid: string): Observable<OperationResult<boolean, ApiDataErrorResponse>> {
    return this.httpServiceAdapter.patch<boolean>(`${this.apiUrl}${ApiRoutes.disableClient}`, { uuid: uuid }).pipe(
      map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {
        console.log(response);
        if (this.httpServiceAdapter.isSuccessResponse<boolean>(response)) {
          return OperationResultService.createSuccess(response.data, response.message);
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

  enableClient(uuid: string): Observable<OperationResult<boolean, ApiDataErrorResponse>> {
    return this.httpServiceAdapter.patch<boolean>(`${this.apiUrl}${ApiRoutes.enableClient}`, { uuid: uuid }).pipe(
      map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {

        console.log(response);
        if (this.httpServiceAdapter.isSuccessResponse<boolean>(response)) {
          return OperationResultService.createSuccess(response.data, response.message);
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


  deleteClient(uuid: string): Observable<OperationResult<boolean, ApiDataErrorResponse>> {
    return this.httpServiceAdapter.delete<boolean>(`${this.apiUrl}${ApiRoutes.deleteClient}?uuid=${uuid}`).pipe(
      map((response: ApiResponse<boolean, ApiDataErrorResponse>) => {

        console.log(response);
        if (this.httpServiceAdapter.isSuccessResponse<boolean>(response)) {
          return OperationResultService.createSuccess(response.data, response.message);
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

  private parseClient(dto: ClientDTO): Client {
    let data = new Client(dto.uuid, dto.email, dto.phoneNumber, dto.username, dto.name, RoleType.CLIENT, dto.status, dto.createdAt);
    return data;
  }

}
