import { Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { IAccountService } from '../communication-interfaces/account-service.interface';
import { HttpClientAdapter } from '../../cross-cutting/communication/http-client-adapter-service/http-client-adapter.service';
import { ConfigService } from '../../cross-cutting/operation-management/configService/config.service';
import { OperationResult } from '../../cross-cutting/communication/model/operation-result';
import { OperationResultService } from '../../cross-cutting/communication/model/operation-result.service';
import { MessageCodeType } from '../../cross-cutting/communication/model/message-code.types';
import { ApiResponse } from '../../cross-cutting/communication/model/api-response';
import { JwtTokenDTO } from '../dtos/jwt-token.dto';
import { UserCredentialsJwt } from '../../view-model/business-entities/user-credentials-jwt';

@Injectable({
  providedIn: 'root'
})

export class AccountService implements IAccountService {
  private apiUrl: string;

  constructor(private httpServiceAdapter: HttpClientAdapter, private configService: ConfigService, private operationResultService: OperationResultService) {
    this.apiUrl = this.configService.getApiBaseUrl() + '/api/v1';

  }

  loginJwtAuth(account: string, password: string): Observable<OperationResult<UserCredentialsJwt, string>> {
    const loginData = {
      account: account,
      password: password
    };

    return this.httpServiceAdapter.post<ApiResponse<JwtTokenDTO, any>>(`${this.apiUrl}/Auth/login`, loginData).pipe(
      map(response => {
        if (response.status == 200) {
          return this.operationResultService.createSuccess(response.data, response.message);
        }
        return this.operationResultService.createFailure(response.data, response.message);
      }),
      catchError(err => {
        return of(this.operationResultService.createFailure(err.message ?? 'Unknown error', MessageCodeType.ERROR));
      })
    );
  }




}

