import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApiResponse } from '../model/api-response';
import { HttpClientService } from '../http-client-service/http-client.service';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageCodeType } from '../model/message-code.types';
import { parseStringToEnum } from '../../helper/enum-utils/enum.utils';
import { ApiVersionType } from '../model/api-version.types';
import { GenericError } from '../model/generic-error';
import { parseTemplate } from '@angular/compiler';


@Injectable({
  providedIn: 'root'
})

export class HttpClientAdapter {

  constructor(private httpClientService: HttpClientService) { }

  post<TData>(uri: string, value: any, isFormData: boolean = false): Observable<ApiResponse<TData, any>> {
    return this.httpClientService.post(uri, value, isFormData).pipe(
      map((response: any) => { // 200 << HttpStatusCode 200 OK
        return {
          status: response.status,
          message: parseStringToEnum(MessageCodeType, response.message.toString()) ?? MessageCodeType.UNKNOWN_ERROR,
          data: response.data as TData,
          version: parseStringToEnum(ApiVersionType, response.version.toString()) ?? ApiVersionType.NO_SPECIFIED,
        };
      }),
      catchError((error: any) => {
        if (error instanceof HttpErrorResponse) {
          if (error.status === 0) { // 0 << HttpStatusCode 0
            const errorResponse: ApiResponse<TData, any> = {
              status: error.status,
              message: MessageCodeType.NO_CONNECTION_WITH_SERVER,
              data: null,
              version: ApiVersionType.NO_SPECIFIED
            }
            return of(errorResponse);
          } else if (error.status === 401) { // HttpStatusCode 401 Unauthorized
            const genericError: GenericError = {
              message: error.error.data.message,
              additionalData: error.error.data.additionalData
            };

            const errorResponse: ApiResponse<TData, any> = {
              status: error.status,
              message: parseStringToEnum(MessageCodeType, error.error.message) ?? MessageCodeType.UNKNOWN_ERROR,
              data: genericError,
              version: ApiVersionType.NO_SPECIFIED
            }
            return of(errorResponse);
          } else if (error.status === 400) { // HttpStatusCode 400 BadRequest
            let codeErrors;
            let parsedErrors: any;
            if (error.error?.errors) {
              parsedErrors = Object.entries(error.error.errors).map(([key, messages]) => ({
                field: key,
                messages: messages as string[],
              }));
              codeErrors = MessageCodeType.DATA_ANNOTATIONS_ERRORS;
            } else {
              parsedErrors = error.error?.data || {};
            }
            const errorResponse: ApiResponse<TData, any> = {
              status: error.status,
              message: error.error.message || codeErrors || MessageCodeType.UNKNOWN_ERROR,
              data: parsedErrors,
              version: error.error?.version || ApiVersionType.NO_SPECIFIED,
            };
            return of(errorResponse);
          }
          throw error;
        }
        throw error;
      })
    );
  }


}

