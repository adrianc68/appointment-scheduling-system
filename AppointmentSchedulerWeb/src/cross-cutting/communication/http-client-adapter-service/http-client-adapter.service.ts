import { Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApiErrorResponse, ApiResponse, ApiSuccessResponse } from '../model/api-response';
import { HttpClientService } from '../http-client-service/http-client.service';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageCodeType } from '../model/message-code.types';
import { parseStringToEnum } from '../../helper/enum-utils/enum.utils';
import { ApiVersionType } from '../model/api-version.types';
import { GenericErrorResponse } from '../model/generic-error.response';
import { ValidationErrorResponse } from '../model/validation-error.response';
import { ServerErrorResponse } from '../model/server-error.response';
import { ApiDataErrorResponse } from '../model/api-response.error';
import { EmptyErrorResponse } from '../model/empty-error.response';
import { InvalidSuccessResponseError } from '../model/exceptions/invalid-success-response.exception';
import { InvalidValueEnumValueException } from '../../../model/dtos/exceptions/invalid-enum.exception';


@Injectable({
  providedIn: 'root'
})

export class HttpClientAdapter {

  constructor(private httpClientService: HttpClientService) { }

  get<TData>(uri: string): Observable<ApiResponse<TData, ApiDataErrorResponse>> {
    return this.httpClientService.get<ApiResponse<TData, ApiDataErrorResponse>>(uri).pipe(
      map((response: ApiResponse<TData, ApiDataErrorResponse>) => this.handleSuccessResponse<TData>(response)),
      catchError((error: HttpErrorResponse) => this.handleErrorResponse(error))
    );
  }

  post<TData>(uri: string, value: any, isFormData: boolean = false): Observable<ApiResponse<TData, ApiDataErrorResponse>> {
    return this.httpClientService.post<ApiResponse<TData, ApiDataErrorResponse>>(uri, value, isFormData).pipe(
      map((response: ApiResponse<TData, ApiDataErrorResponse>) => this.handleSuccessResponse<TData>(response)),
      catchError((error: HttpErrorResponse) => this.handleErrorResponse(error))
    );
  }

  put<TData>(uri: string, value: any, isFormData: boolean = false): Observable<ApiResponse<TData, ApiDataErrorResponse>> {
    return this.httpClientService.put<ApiResponse<TData, ApiDataErrorResponse>>(uri, value, isFormData).pipe(
      map((response: ApiResponse<TData, ApiDataErrorResponse>) => this.handleSuccessResponse<TData>(response)),
      catchError((error: HttpErrorResponse) => this.handleErrorResponse(error))
    );
  }

  patch<TData>(uri: string, value: any, isFormData: boolean = false): Observable<ApiResponse<TData, ApiDataErrorResponse>> {
    return this.httpClientService.patch<ApiResponse<TData, ApiDataErrorResponse>>(uri, value, isFormData).pipe(
      map((response: ApiResponse<TData, ApiDataErrorResponse>) => this.handleSuccessResponse<TData>(response)),
      catchError((error: HttpErrorResponse) => this.handleErrorResponse(error))
    );
  }

  delete<TData>(uri: string): Observable<ApiResponse<TData, ApiDataErrorResponse>> {
    return this.httpClientService.delete<ApiResponse<TData, ApiDataErrorResponse>>(uri).pipe(
      map((response: ApiResponse<TData, ApiDataErrorResponse>) => this.handleSuccessResponse<TData>(response)),
      catchError((error: HttpErrorResponse) => this.handleErrorResponse(error))
    );
  }

  isSuccessResponse<TData>(response: ApiResponse<TData, ApiDataErrorResponse>): response is ApiSuccessResponse<TData> {
    return response.status >= 200 && response.status < 300;
  }

  private handleErrorResponse(error: HttpErrorResponse): Observable<ApiErrorResponse<ApiDataErrorResponse>> {
    if (error instanceof HttpErrorResponse) {
      switch (error.status) {
        case 0: // No connection with server
          const emptyError: EmptyErrorResponse = {};
          return of(this.createErrorResponse(error.status, MessageCodeType.NO_CONNECTION_WITH_SERVER, emptyError));
        case 401: // Unauthorized
        case 409: // Conflict
          const parsedMessage = parseStringToEnum(MessageCodeType, error.error.message);

          if (parsedMessage === undefined) {
            throw new InvalidValueEnumValueException("Not possible to parse MessageCodeType");
          }
          const genericError: GenericErrorResponse = {
            message: parsedMessage,
            additionalData: error.error.data?.additionalData,
          };
          return of(this.createErrorResponse(error.status, parsedMessage, genericError));
        case 400: // Bad Request
          const validationErrors: ValidationErrorResponse[] = this.parseBadRequestErrors(error.error);
          const errorMessage = error.error.message || MessageCodeType.DATA_ANNOTATIONS_ERRORS;
          return of(this.createErrorResponse(error.status, errorMessage, validationErrors));
        case 500: // Internal Server Error
          const internalError: ServerErrorResponse = {
            details: error.error.data!.details,
            error: parseStringToEnum(MessageCodeType, error.error.data.error) ?? MessageCodeType.SERVER_ERROR,
            identifier: error.error.data.identifier,
            message: error.error.data.message
          }
          return of(this.createErrorResponse(error.status, parseStringToEnum(MessageCodeType, error.error.message) || MessageCodeType.INTERNAL_SERVER_ERROR, internalError));

        default:
          throwError(() => error);
      }
    }
    throw error;
  }


  private handleSuccessResponse<TData>(response: ApiResponse<TData, ApiDataErrorResponse>): ApiSuccessResponse<TData> {
    if (this.isSuccessResponse(response)) {
      return {
        status: response.status,
        message: parseStringToEnum(MessageCodeType, response.message.toString()) ?? MessageCodeType.UNKNOWN_ERROR,
        data: response.data as TData,
        version: parseStringToEnum(ApiVersionType, response.version.toString()) ?? ApiVersionType.NO_SPECIFIED,
      }
    }
    throw new InvalidSuccessResponseError("Invalid sucess response");
  }

  private createErrorResponse(status: number, message: MessageCodeType, data: ApiDataErrorResponse, version: ApiVersionType = ApiVersionType.NO_SPECIFIED): ApiErrorResponse<ApiDataErrorResponse> {
    return {
      status,
      message,
      data,
      version,
    };
  }

  private parseBadRequestErrors(error: any): any {
    if (error?.errors) {
      return Object.entries(error.errors).map(([key, messages]) => ({
        field: key,
        messages: messages as string[],
      }));
    }
    return error?.data || {};
  }

}

