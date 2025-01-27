import { Injectable } from '@angular/core';
import { IAccountService } from '../communication-interfaces/account-service.interface';
import { HttpClientAdapter } from '../../cross-cutting/communication/http-client-adapter-service/http-client-adapter.service';
import { ConfigService } from '../../cross-cutting/operation-management/configService/config.service';
import { OperationResultService } from '../../cross-cutting/communication/model/operation-result.service';
import { ApiVersionRoute } from '../../cross-cutting/operation-management/model/api-routes.constants';
import { Observable } from 'rxjs';
import { ApiDataErrorResponse } from '../../cross-cutting/communication/model/api-response.error';
import { OperationResult } from '../../cross-cutting/communication/model/operation-result.response';
import { AccountDataDTO } from '../dtos/account-data.dto';

@Injectable({
  providedIn: 'root'
})

export class AccountService implements IAccountService {
  private apiUrl: string;

  constructor(private httpServiceAdapter: HttpClientAdapter, private configService: ConfigService, private operationResultService: OperationResultService) {
    this.apiUrl = this.configService.getApiBaseUrl() + ApiVersionRoute.v1;
  }








}

