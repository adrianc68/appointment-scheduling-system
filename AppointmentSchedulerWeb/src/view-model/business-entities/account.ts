import { AccountStatusType } from "./types/account-status.types";
import { RoleType } from "./types/role.types";

export interface AccountData {
  uuid: string;
  email: string;
  phoneNumber: string;
  username: string;
  name: string;
  role: RoleType;
  status: AccountStatusType;
  createdAt: Date;
}
