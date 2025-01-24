import { Observable } from "rxjs";

export interface IHttpClient {
  Get(uri: string): Observable<string>;
  Post(uri: string, value: object): Observable<string>;
  Patch(uri: string, value: object): Observable<string>;
  Put(uri: string, value: object): Observable<string>;
  Delete(uri: string): Observable<string>;
}
