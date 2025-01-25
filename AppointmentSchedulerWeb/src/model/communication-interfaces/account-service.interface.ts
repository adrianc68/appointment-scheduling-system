import { Observable } from "rxjs";
import { JwtTokenDTO } from "../../view-model/dtos/jwt-token.dto";
import { OperationResult } from "../../cross-cutting/communication/model/operation-result";

export interface IAccountService {
  loginJwtAuth(account: string, password: string): Observable<OperationResult<JwtTokenDTO, string>>;
}
