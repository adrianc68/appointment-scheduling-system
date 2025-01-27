import { Observable } from "rxjs";
import { OperationResult } from "../../cross-cutting/communication/model/operation-result.response";
import { AccountDataDTO } from "../dtos/account-data.dto";
import { ApiDataErrorResponse } from "../../cross-cutting/communication/model/api-response.error";

export interface IAccountService {

  //getAccountData(): Observable<OperationResult<AccountDataDTO, ApiDataErrorResponse>>;

}
