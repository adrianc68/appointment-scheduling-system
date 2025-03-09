import { AccountStatusType } from "../../view-model/business-entities/types/account-status.types";
import { RoleType } from "../../view-model/business-entities/types/role.types";

export interface AccountDataDTO {
  uuid: string;
  email: string;
  phoneNumber: string;
  username: string;
  name: string;
  role: RoleType;
  status: AccountStatusType;
  createdAt: Date;
}
