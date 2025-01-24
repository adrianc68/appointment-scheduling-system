import { Injectable } from '@angular/core';
import { IHttpClient } from './http-client.interface';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class HttpClientService implements IHttpClient {

  constructor(private http: HttpClient) { }

  Get(uri: string): Observable<string> {
    return this.http.get<string>(uri);
  }

  Post(uri: string, value: object | FormData, isFormData: boolean = false): Observable<string> {
    const headers = this.createHeaders(isFormData);
    return this.http.post<string>(uri, value, { headers });
  }

  Patch(uri: string, value: object | FormData, isFormData: boolean = false): Observable<string> {
    const headers = this.createHeaders(isFormData);
    return this.http.patch<string>(uri, value, { headers });
  }
  Put(uri: string, value: object | FormData, isFormData: boolean = false): Observable<string> {
    const headers = this.createHeaders(isFormData);
    return this.http.put<string>(uri, value, { headers });
  }

  Delete(uri: string): Observable<string> {
    return this.http.delete<string>(uri);
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
