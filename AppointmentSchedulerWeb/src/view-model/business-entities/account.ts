import { AccountStatusType } from "./types/account-status.types";
import { RoleType } from "./types/role.types";

export class AccountData {
  uuid: string;
  email: string;
  phoneNumber: string;
  username: string;
  name: string;
  role: RoleType;
  status: AccountStatusType;
  createdAt: Date;

  constructor(uuid: string, email: string, phoneNumber: string, username: string, name: string, role: RoleType, status: AccountStatusType, createdAt: Date) {
    this.uuid = uuid;
    this.email = email;
    this.phoneNumber = phoneNumber;
    this.username = username;
    this.name = name;
    this.role = role;
    this.status = status;
    this.createdAt = createdAt;
  }
}
