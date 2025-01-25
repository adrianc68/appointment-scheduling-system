import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApiResponse } from '../model/api-response';
import { HttpClientService } from '../http-client-service/http-client.service';
import { HttpErrorResponse } from '@angular/common/http';


@Injectable({
  providedIn: 'root'
})
export class HttpClientAdapter {

  constructor(private httpClientService: HttpClientService) { }

  post<TData>(uri: string, value: any, isFormData: boolean = false): Observable<ApiResponse<TData, any>> {
    return this.httpClientService.post(uri, value, isFormData).pipe(
      map((response: any) => {
        let parsedResponse: ApiResponse<TData, any>;

        if (typeof response === 'object') {
          parsedResponse = response;
        } else {
          parsedResponse = JSON.parse(response);
        }

        return {
          status: parsedResponse.status,
          message: parsedResponse.message,
          data: parsedResponse.data as TData,
          version: parsedResponse.version,
        };
      }),
      catchError((error: any) => {
        if (error instanceof HttpErrorResponse) {
          let parsedErrors: any;


          if (error.error?.errors) {
            parsedErrors = Object.entries(error.error.errors).map(([key, messages]) => ({
              field: key,
              messages: messages as string[],
            }));
          } else {
            parsedErrors = error.error?.data || {};
          }

          const errorResponse: ApiResponse<TData, any> = {
            status: error.status,
            message: error.error.message || error.error.title,
            data: parsedErrors,
            version: error.error?.version || "v1",
          };

          return of(errorResponse);
        }
        throw error;
      })
    );
  }


}

