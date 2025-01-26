import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, of } from 'rxjs';
import { UserCredentialsJwt } from '../../../view-model/business-entities/user-credentials-jwt';
import { LocalStorageService } from '../local-storage/local-storage.service';
import { OperationResult } from '../../communication/model/operation-result';
import { MessageCodeType } from '../../communication/model/message-code.types';
import { OperationResultService } from '../../communication/model/operation-result.service';
import { HttpClientAdapter } from '../../communication/http-client-adapter-service/http-client-adapter.service';
import { ApiResponse } from '../../communication/model/api-response';
import { JwtTokenDTO } from '../../../model/dtos/jwt-token.dto';
import { ConfigService } from '../../operation-management/configService/config.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  //private currentUserSubject: BehaviorSubject<UserCredentialsJwt>;
  //public currenUser: Observable<any>;
  private apiUrl: string;

  constructor(private httpServiceAdapter: HttpClientAdapter, private operationResultService: OperationResultService, private localStorageService: LocalStorageService, private configService: ConfigService) {

    this.apiUrl = this.configService.getApiBaseUrl() + '/api/v1';
    //this.currenUserSubject = new BehaviorSubject<any>(JSON.parse(localStorageService.getItem("jwtToken")));
    //this.currentUser = this.currentUserSub
  }

  loginJwt(account: string, password: string): Observable<OperationResult<boolean, any>> {
    const loginData = {
      account: account,
      password: password
    };

    return this.httpServiceAdapter.post<ApiResponse<JwtTokenDTO, any>>(`${this.apiUrl}/Auth/login`, loginData).pipe(
      map(response => {

        if (response.status == 200) { // 200 < Http Status Code 200 OK
          this.localStorageService.setItem("jwtToken", response.data.token)
          return this.operationResultService.createSuccess(true, response.message);
        }
        return this.operationResultService.createFailure(response.data, response.message);
      }),
      catchError(err => {
        let codeError = MessageCodeType.UNKNOWN_ERROR;
        if (err.status == 500) { // 500 < Http Status Code 500 Internal Server Error
          codeError = MessageCodeType.SERVER_ERROR;
        }
        return of(this.operationResultService.createFailure(err.message, codeError));
      })
    );
  }


  logout(): void {
    this.localStorageService.removeItem("jwtToken");

  }



}
