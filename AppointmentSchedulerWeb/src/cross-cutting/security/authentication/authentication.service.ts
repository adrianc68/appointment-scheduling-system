import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, of } from 'rxjs';
import { LocalStorageService } from '../local-storage/local-storage.service';
import { OperationResult } from '../../communication/model/operation-result.response';
import { MessageCodeType } from '../../communication/model/message-code.types';
import { OperationResultService } from '../../communication/model/operation-result.service';
import { HttpClientAdapter } from '../../communication/http-client-adapter-service/http-client-adapter.service';
import { ApiResponse, ApiSuccessResponse } from '../../communication/model/api-response';
import { JwtTokenDTO } from '../../../model/dtos/jwt-token.dto';
import { ConfigService } from '../../operation-management/configService/config.service';
import { ApiDataErrorResponse } from '../../communication/model/api-response.error';
import { UserCredentialsJwt } from '../../../view-model/business-entities/user-credentials-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private currentTokenData: BehaviorSubject<UserCredentialsJwt | null>;

  private apiUrl: string;

  constructor(private httpServiceAdapter: HttpClientAdapter, private operationResultService: OperationResultService, private localStorageService: LocalStorageService, private configService: ConfigService) {

    this.apiUrl = this.configService.getApiBaseUrl() + '/api/v1';
    const storedToken = this.localStorageService.getItem<string>("token");
    const storedExpiration = this.localStorageService.getItem<string>("expirationToken");
    this.currentTokenData = new BehaviorSubject<UserCredentialsJwt | null>(storedToken && storedExpiration ? this.parseJwtToken(storedToken, storedExpiration) : null);
  }

  get currentToken$(): Observable<UserCredentialsJwt | null> {
    return this.currentTokenData.asObservable();
  }

  isAuthenticated(): boolean {
    const credentials = this.currentTokenData.value;
    return !!credentials && credentials.expiration.getTime() > Date.now();
  }

  loginJwt(account: string, password: string): Observable<OperationResult<boolean, ApiDataErrorResponse>> {
    const loginData = {
      account: account,
      password: password
    };

    return this.httpServiceAdapter.post<JwtTokenDTO>(`${this.apiUrl}/Auth/login`, loginData).pipe(
      map((response: ApiResponse<JwtTokenDTO, ApiDataErrorResponse>) => {
        if (this.isSuccessResponse<JwtTokenDTO>(response)) { // 200 < Http Status Code 200 OK
          this.localStorageService.setItem("token", response.data.token);
          this.localStorageService.setItem("expiration_token", response.data.expiration);
          this.currentTokenData.next(this.parseJwtToken(response.data.token, response.data.expiration.toString()));
          return this.operationResultService.createSuccess<boolean>(true, response.message);
        }
        return this.operationResultService.createFailure<ApiDataErrorResponse>(response.data, response.message);
      }),
      catchError((err) => {
        let codeError = MessageCodeType.UNKNOWN_ERROR;
        if (err.status == 500) { // 500 < Http Status Code 500 Internal Server Error
          codeError = MessageCodeType.SERVER_ERROR;
        }
        return of(this.operationResultService.createFailure<ApiDataErrorResponse>(err.error, codeError));
      })
    );
  }

  logout(): void {
    this.localStorageService.removeItem("token");
    this.localStorageService.removeItem("expiration_token");
    this.currentTokenData.next(null);
  }


  private isSuccessResponse<TData>(response: ApiResponse<TData, ApiDataErrorResponse>): response is ApiSuccessResponse<TData> {
    return response.status >= 200 && response.status < 300;
  }

  private parseJwtToken(storedToken: string, storedExpiration: string): UserCredentialsJwt {
    return {
      token: storedToken,
      expiration: new Date(storedExpiration),
    };
  }



}
