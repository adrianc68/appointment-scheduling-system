import { Injectable } from '@angular/core';
import { IHttpClient } from './http-client.interface';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthenticationService } from '../../security/authentication/authentication.service';

@Injectable({
  providedIn: 'root'
})

export class HttpClientService implements IHttpClient {

  constructor(private http: HttpClient) { }

  get<T>(uri: string): Observable<T> {
    return this.http.get<T>(uri);
  }

  post<T>(uri: string, value: object | FormData, isFormData: boolean = false): Observable<T> {
    const headers = this.createHeaders(isFormData);
    return this.http.post<T>(uri, value, { headers });
  }

  patch<T>(uri: string, value: object | FormData, isFormData: boolean = false): Observable<T> {
    const headers = this.createHeaders(isFormData);
    return this.http.patch<T>(uri, value, { headers });
  }

  put<T>(uri: string, value: object | FormData, isFormData: boolean = false): Observable<T> {
    const headers = this.createHeaders(isFormData);
    return this.http.put<T>(uri, value, { headers });
  }

  delete<T>(uri: string): Observable<T> {
    return this.http.delete<T>(uri);
  }

  private createHeaders(isFormData: boolean): HttpHeaders {
    if (isFormData) {
      return new HttpHeaders();
    }
    return new HttpHeaders({
      "Content-Type": "application/json"
    })

  }



}
