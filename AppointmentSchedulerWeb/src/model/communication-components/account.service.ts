import { Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { IAccountService } from '../communication-interfaces/account-service.interface';
import { HttpClientAdapter } from '../../cross-cutting/communication/http-client-adapter-service/http-client-adapter.service';
import { ConfigService } from '../../cross-cutting/operation-management/configService/config.service';
import { OperationResultService } from '../../cross-cutting/communication/model/operation-result.service';

@Injectable({
  providedIn: 'root'
})

export class AccountService implements IAccountService {
  private apiUrl: string;

  constructor(private httpServiceAdapter: HttpClientAdapter, private configService: ConfigService, private operationResultService: OperationResultService) {
    this.apiUrl = this.configService.getApiBaseUrl() + '/api/v1';

  }






}

