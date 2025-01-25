import { MessageCodeType } from "../../communication/model/message-code.types";


export function parseMessageCode(errorMessage: string): MessageCodeType {
  const code = errorMessage as unknown;

  if (Object.values(MessageCodeType).includes(code as MessageCodeType)) {
    return code as MessageCodeType;
  }

  return MessageCodeType.SERVER_ERROR;
}
