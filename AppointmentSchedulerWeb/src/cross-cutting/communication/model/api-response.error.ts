import { EmptyErrorResponse } from "./empty-error.response";
import { GenericErrorResponse } from "./generic-error.response";
import { ServerErrorResponse } from "./server-error.response";
import { ValidationErrorResponse } from "./validation-error.response";

export type ApiDataErrorResponse = GenericErrorResponse | ValidationErrorResponse[] | ServerErrorResponse | EmptyErrorResponse;
