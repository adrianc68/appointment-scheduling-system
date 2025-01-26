import { Observable } from "rxjs";
import { OperationResult } from "../../cross-cutting/communication/model/operation-result";
import { UserCredentialsJwt } from "../../view-model/business-entities/user-credentials-jwt";

export interface IAccountService {
  loginJwtAuth(account: string, password: string): Observable<OperationResult<UserCredentialsJwt, string>>;
}
