import { EmptyErrorResponse } from "./empty-error.response";
import { GenericErrorResponse } from "./generic-error.response";
import { ServerErrorResponse } from "./server-error.response";
import { ValidationErrorResponse } from "./validation-error.response";

export type ApiDataErrorResponse = GenericErrorResponse | ValidationErrorResponse[] | ServerErrorResponse | EmptyErrorResponse;


export function isGenericErrorResponse(error: any): error is GenericErrorResponse {
  return (error as GenericErrorResponse).additionalData !== undefined;
}

export function isValidationErrorResponse(error: any): error is ValidationErrorResponse[] {
  return Array.isArray(error) && error.every(e => (e as ValidationErrorResponse).field !== undefined);
}

export function isServerErrorResponse(error: any): error is ServerErrorResponse {
  return (error as ServerErrorResponse).identifier !== undefined;
}

export function isEmptyErrorResponse(error: any): error is EmptyErrorResponse {
  return error && typeof error === 'object' && Object.keys(error).length === 0;
}

