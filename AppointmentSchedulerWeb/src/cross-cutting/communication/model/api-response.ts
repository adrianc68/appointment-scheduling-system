import { ApiVersionType } from "./api-version.types";
import { MessageCodeType } from "./message-code.types";

export interface ApiResponse<TData, TError> {
  status: number;
  message: MessageCodeType;
  data: TData | TError,
  version: ApiVersionType;
}
