import { Observable } from "rxjs";

export interface IHttpClient {
  get<T>(uri: string): Observable<T>;
  post<T>(uri: string, value: object): Observable<T>;
  patch<T>(uri: string, value: object): Observable<T>;
  put<T>(uri: string, value: object): Observable<T>;
  delete<T>(uri: string): Observable<T>;
}
