import { AccountData } from "./account";
import { AccountStatusType } from "./types/account-status.types";
import { RoleType } from "./types/role.types";

export class Client extends AccountData {

constructor(uuid: string, email: string, phoneNumber: string, username: string, name: string, role: RoleType, status: AccountStatusType, createdAt: Date) {
    super(uuid, email, phoneNumber, username, name, role, status, createdAt);
  }

}
