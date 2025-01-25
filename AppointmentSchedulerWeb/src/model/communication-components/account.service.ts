import { Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { IAccountService } from '../communication-interfaces/account-service.interface';
import { HttpClientAdapter } from '../../cross-cutting/communication/http-client-adapter-service/http-client-adapter.service';
import { JwtTokenDTO } from '../../view-model/dtos/jwt-token.dto';
import { ConfigService } from '../../cross-cutting/operation-management/configService/config.service';
import { OperationResult } from '../../cross-cutting/communication/model/operation-result';
import { OperationResultService } from '../../cross-cutting/communication/model/operation-result.service';
import { MessageCodeType } from '../../cross-cutting/communication/model/message-code.types';
import { ApiResponse } from '../../cross-cutting/communication/model/api-response';
import { parseMessageCode } from '../../cross-cutting/helper/enum-utils/message-code.utils';

@Injectable({
  providedIn: 'root'
})

export class AccountService implements IAccountService {
  private apiUrl: string;

  constructor(private httpService: HttpClientAdapter, private configService: ConfigService, private operationResultService: OperationResultService) {
    this.apiUrl = this.configService.getApiBaseUrl() + '/api/v1';

  }

  loginJwtAuth(account: string, password: string): Observable<OperationResult<JwtTokenDTO, string>> {
    const loginData = {
      account: account,
      password: password
    };

    return this.httpService.post<ApiResponse<JwtTokenDTO, any>>(`${this.apiUrl}/Auth/login`, loginData).pipe(
      map(response => {

        console.log("******");
        console.log(response);

        if (response.status == 200) {
          return this.operationResultService.createSuccess(response.data, parseMessageCode(response.message));
        }
        return this.operationResultService.createFailure(response.data, parseMessageCode(response.message));
      }),
      catchError(err => {
        return of(this.operationResultService.createFailure(err.message || 'Unknown error', MessageCodeType.ERROR));
      })
    );
  }




}

