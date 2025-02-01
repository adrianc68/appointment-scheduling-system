import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, of, throwError } from 'rxjs';
import { LocalStorageService } from '../local-storage/local-storage.service';
import { OperationResult } from '../../communication/model/operation-result.response';
import { MessageCodeType } from '../../communication/model/message-code.types';
import { OperationResultService } from '../../communication/model/operation-result.service';
import { HttpClientAdapter } from '../../communication/http-client-adapter-service/http-client-adapter.service';
import { ApiResponse } from '../../communication/model/api-response';
import { JwtTokenDTO } from '../../../model/dtos/jwt-token.dto';
import { ConfigService } from '../../operation-management/configService/config.service';
import { ApiDataErrorResponse } from '../../communication/model/api-response.error';
import { UserCredentialsJwt } from '../../../view-model/business-entities/user-credentials-jwt';
import { ApiRoutes, ApiVersionRoute } from '../../operation-management/model/api-routes.constants';
import { HttpErrorResponse } from '@angular/common/http';
import { AccountDataDTO } from '../../../model/dtos/account-data.dto';
import { AccountData } from '../../../view-model/business-entities/account';
import { parseStringToEnum } from '../../helper/enum-utils/enum.utils';
import { RoleType } from '../../../view-model/business-entities/types/role.types';
import { AccountStatusType } from '../../../view-model/business-entities/types/account-status.types';
import { InvalidValueEnumValueException } from '../../../model/dtos/exceptions/invalid-enum.exception';
import { Router } from '@angular/router';
import { WebRoutes } from '../../operation-management/model/web-routes.constants';
import { LocalStorageKeys } from '../model/local-storage-keys.constants';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private currentTokenData: BehaviorSubject<UserCredentialsJwt | null>;
  private currentUserData: BehaviorSubject<AccountData | null>;

  constructor(private httpServiceAdapter: HttpClientAdapter, private localStorageService: LocalStorageService, private configService: ConfigService, private router: Router) {
    let tokenData = this.localStorageService.getItem<UserCredentialsJwt>(LocalStorageKeys.token);
    let accountData = this.localStorageService.getItem<AccountData>(LocalStorageKeys.account_data);

    if (tokenData?.expiration) {
      tokenData.expiration = new Date(tokenData.expiration);
    }

    this.currentTokenData = new BehaviorSubject<UserCredentialsJwt | null>(tokenData);
    this.currentUserData = new BehaviorSubject<AccountData | null>(accountData);
  }

  getCredentials(): Observable<UserCredentialsJwt | null> {
    return this.currentTokenData.asObservable();
  }

  getAccountData(): Observable<AccountData | null> {
    return this.currentUserData.asObservable();
  }

  isAuthenticated(): Observable<boolean> {
    return this.currentTokenData.pipe(
      map((credentials: UserCredentialsJwt | null) => {



        return !!credentials && credentials.expiration.getTime() > Date.now();
      })
    )
  }

  getAccountDataFromServer(): Observable<OperationResult<boolean, ApiDataErrorResponse>> {
    return this.httpServiceAdapter.get<AccountDataDTO>(`${this.configService.getApiBaseUrl()}${ApiVersionRoute.v1}${ApiRoutes.getAccountData}`).pipe(
      map((response: ApiResponse<AccountDataDTO, ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<AccountDataDTO>(response)) {
          const accountData = this.parseAccountData(response.data);
          this.currentUserData.next(accountData);
          this.localStorageService.setItem<AccountData>(LocalStorageKeys.account_data, accountData);
          return OperationResultService.createSuccess(true, response.message);
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

  loginJwt(account: string, password: string): Observable<OperationResult<boolean, ApiDataErrorResponse>> {
    const loginData = {
      account: account,
      password: password
    };

    return this.httpServiceAdapter.post<JwtTokenDTO>(`${this.configService.getApiBaseUrl()}${ApiVersionRoute.v1}${ApiRoutes.login}`, loginData).pipe(
      map((response: ApiResponse<JwtTokenDTO, ApiDataErrorResponse>) => {
        if (this.httpServiceAdapter.isSuccessResponse<JwtTokenDTO>(response)) { // 200 < Http Status Code 200 OK
          let tokenDataa = {
            accessToken: response.data.token,
            expiration: new Date(response.data.expiration)
          };
          this.localStorageService.setItem<UserCredentialsJwt>(LocalStorageKeys.token, tokenDataa);
          this.currentTokenData.next(tokenDataa);
          return OperationResultService.createSuccess<boolean>(true, response.message);
        }
        return OperationResultService.createFailure<ApiDataErrorResponse>(response.data, response.message);
      }),
      catchError((err) => {
        if (err instanceof HttpErrorResponse) {
          let codeError = MessageCodeType.UNKNOWN_ERROR;
          if (err.status == 500) { // 500 < Http Status Code 500 Internal Server Error
            codeError = MessageCodeType.SERVER_ERROR;
          }
          return of(OperationResultService.createFailure<ApiDataErrorResponse>(err.error, codeError));
        }
        return throwError(() => err);
      })
    );
  }

  logout(): void {
    this.localStorageService.removeItem(LocalStorageKeys.token);
    this.localStorageService.removeItem(LocalStorageKeys.account_data);
    this.currentTokenData.next(null);
    this.currentUserData.next(null);
    this.router.navigate([WebRoutes.root]);
  }

  private parseAccountData(accountData: AccountDataDTO): AccountData {
    let data: AccountData = {
      uuid: accountData.uuid,
      email: accountData.email,
      phoneNumber: accountData.phoneNumber,
      username: accountData.username,
      name: accountData.name,
      role: parseStringToEnum(RoleType, accountData.role.toString()) ?? (() => { throw new InvalidValueEnumValueException("Invalid role: Unable to parse role to enum."); })(),
      status: parseStringToEnum(AccountStatusType, accountData.status.toString()) ?? (() => { throw new InvalidValueEnumValueException("Invalid account status type: Unable to parse role to enum"); })(),
      createdAt: accountData.createdAt
    };
    return data;
  }

}
