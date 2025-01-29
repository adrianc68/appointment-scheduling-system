import { EmptyErrorResponse } from "./empty-error.response";
import { GenericErrorResponse } from "./generic-error.response";
import { ServerErrorResponse } from "./server-error.response";
import { ValidationErrorResponse } from "./validation-error.response";

export type ApiDataErrorResponse = GenericErrorResponse | ValidationErrorResponse[] | ServerErrorResponse | EmptyErrorResponse;


export function isGenericErrorResponse(error: any): error is GenericErrorResponse {
  const expectedKeys = ["message", "additionalData"];
  const actualKeys = Object.keys(error);
  return actualKeys.every(key => expectedKeys.includes(key));
}

export function isValidationErrorResponse(error: any): error is ValidationErrorResponse[] {
  return Array.isArray(error) && error.every(e => (e as ValidationErrorResponse).field !== undefined);
}

export function isServerErrorResponse(error: any): error is ServerErrorResponse {
  const expectedKeys = ["details", "error", "identifier", "message"];
  const actualKeys = Object.keys(error);
  return actualKeys.every(key => expectedKeys.includes(key));
}

export function isEmptyErrorResponse(error: any): error is EmptyErrorResponse {
  return error && typeof error === 'object' && Object.keys(error).length === 0;
}

