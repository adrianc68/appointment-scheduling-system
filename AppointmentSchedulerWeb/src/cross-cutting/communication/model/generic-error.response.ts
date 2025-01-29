import { MessageCodeType } from "./message-code.types";

export interface GenericErrorResponse {
  message: MessageCodeType,
  additionalData?: { [key: string]: any };
}
