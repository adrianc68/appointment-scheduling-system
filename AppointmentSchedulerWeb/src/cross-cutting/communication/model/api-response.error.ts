import { EmptyErrorResponse } from "./empty-error.response";
import { GenericErrorResponse } from "./generic-error.response";
import { ServerErrorResponse } from "./server-error.response";
import { ValidationErrorResponse } from "./validation-error.response";

export type ApiDataErrorResponse = GenericErrorResponse | ValidationErrorResponse[] | ServerErrorResponse | EmptyErrorResponse;


export function isGenericErrorResponse(error: any): error is GenericErrorResponse {
  if (!error || typeof error !== "object" || Array.isArray(error)) return false;
  const expectedKeys = ["message", "additionalData"];
  let data = expectedKeys.every(key => key in error && error[key] !== undefined);
  return data;
}

export function isValidationErrorResponse(error: any): error is ValidationErrorResponse[] {
  return Array.isArray(error) &&
    error.length > 0 &&
    error.every((e: any) =>
      typeof e === "object" &&
      e !== null &&
      "field" in e && typeof e.field === "string" &&
      "messages" in e && Array.isArray(e.messages) &&
      e.messages.every((m: any) => typeof m === "string")
    );
}

export function isServerErrorResponse(error: any): error is ServerErrorResponse {
  const expectedKeys = ["details", "error", "identifier", "message"];
  let data = expectedKeys.every(key => key in error && error[key] !== undefined);
  return data;
}

export function isEmptyErrorResponse(error: any): error is EmptyErrorResponse {
  return error && typeof error === 'object' && Object.keys(error).length === 0;
}

