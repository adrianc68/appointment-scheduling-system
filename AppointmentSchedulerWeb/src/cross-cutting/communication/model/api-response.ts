import { ApiVersionType } from "./api-version.types";
import { MessageCodeType } from "./message-code.types";

export interface ApiSuccessResponse<TData> {
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

export type ApiResponse<TData, TError> = ApiSuccessResponse<TData> | ApiErrorResponse<TError>;
