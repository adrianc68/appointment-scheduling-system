import { MessageCodeType } from "./message-code.types";

export interface ServerErrorResponse {
  details: string;
  error: MessageCodeType;
  identifier: string;
  message: string;
}
