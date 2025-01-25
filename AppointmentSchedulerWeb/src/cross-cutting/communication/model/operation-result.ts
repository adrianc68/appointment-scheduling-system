import { MessageCodeType } from "./message-code.types";

export interface OperationResult<TData, TError> {
  isSuccessful: boolean;
  code: MessageCodeType;
  result?: TData;
  error?: TError;
  errors?: TError[];
}
