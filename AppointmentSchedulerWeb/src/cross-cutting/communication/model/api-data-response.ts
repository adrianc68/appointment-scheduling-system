import { ApiVersionType } from "./api-version.types";
import { MessageCodeType } from "./message-code.types";

export interface ApiResponse<TData> {
  status: number;
  message: MessageCodeType;
  data: TData,
  version: ApiVersionType;
}

export interface ApiErrorResponse<TError> {
  status: number;
  message: MessageCodeType;
  data: TError,
  version: ApiVersionType;
}

