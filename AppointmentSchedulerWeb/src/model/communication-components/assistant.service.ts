
import { Injectable } from "@angular/core";
import { HttpClientAdapter } from "../../cross-cutting/communication/http-client-adapter-service/http-client-adapter.service";
import { ConfigService } from "../../cross-cutting/operation-management/configService/config.service";
import { ApiRoutes, ApiVersionRoute } from "../../cross-cutting/operation-management/model/api-routes.constants";
import { OperationResult } from "../../cross-cutting/communication/model/operation-result.response";
import { ApiDataErrorResponse } from "../../cross-cutting/communication/model/api-response.error";
import { catchError, map, Observable, of, throwError } from "rxjs";
import { ApiResponse } from "../../cross-cutting/communication/model/api-response";
import { HttpErrorResponse } from "@angular/common/http";
import { MessageCodeType } from "../../cross-cutting/communication/model/message-code.types";
import { OperationResultService } from "../../cross-cutting/communication/model/operation-result.service";
import { RoleType } from "../../view-model/business-entities/types/role.types";
import { Assistant } from "../../view-model/business-entities/assistant";
import { AssistantDTO } from "../dtos/assistant.dto";

@Injectable({
  providedIn: 'root'
})

export class AssistantService {
  private apiUrl: string;

  constructor(private httpServiceAdapter: HttpClientAdapter, private configService: ConfigService) {
    this.apiUrl = this.configService.getApiBaseUrl() + ApiVersionRoute.v1;
  }

  getAssistantList(): Observable<OperationResult<Assistant[], ApiDataErrorResponse>> {
    return this.httpServiceAdapter.get<AssistantDTO[]>(`${this.apiUrl}${ApiRoutes.getAllAssistants}`).pipe(
      map((response: ApiResponse<AssistantDTO[], ApiDataErrorResponse>) => {
        console.log(response);
        if (this.httpServiceAdapter.isSuccessResponse<AssistantDTO[]>(response)) {
          const assistants: Assistant[] = response.data.map(dto => this.parseAssistant(dto));
          return OperationResultService.createSuccess(assistants, response.message);
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


  editAssistant(assistant: Assistant): Observable<OperationResult<boolean, ApiDataErrorResponse>> {
    return this.httpServiceAdapter.put<boolean>(`${this.apiUrl}${ApiRoutes.editAssistant}`, assistant).pipe(
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

  disableAssistant(uuid: string): Observable<OperationResult<boolean, ApiDataErrorResponse>> {
    return this.httpServiceAdapter.patch<boolean>(`${this.apiUrl}${ApiRoutes.disableAssistant}`, { uuid: uuid }).pipe(
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

  enableAssistant(uuid: string): Observable<OperationResult<boolean, ApiDataErrorResponse>> {
    return this.httpServiceAdapter.patch<boolean>(`${this.apiUrl}${ApiRoutes.enableAssistant}`, { uuid: uuid }).pipe(
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


  deleteAssistant(uuid: string): Observable<OperationResult<boolean, ApiDataErrorResponse>> {
    return this.httpServiceAdapter.delete<boolean>(`${this.apiUrl}${ApiRoutes.deleteAssistant}?uuid=${uuid}`).pipe(
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


  private parseAssistant(dto: AssistantDTO): Assistant {
    let data = new Assistant(dto.uuid, dto.email, dto.phoneNumber, dto.username, dto.name, RoleType.ASSISTANT, dto.status, dto.createdAt);
    return data;
  }

}
