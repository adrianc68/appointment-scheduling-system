import { Injectable } from '@angular/core';
import { OperationResult } from '../model/operation-result.response';
import { ApiDataErrorResponse, isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../model/api-response.error';
import { MessageCodeType } from '../model/message-code.types';
import { getStringEnumKeyByValue } from '../../helper/enum-utils/enum.utils';

@Injectable({
  providedIn: 'root'
})
export class ErrorUIService {

  constructor() { }

  public handleError<T>(response: OperationResult<T, ApiDataErrorResponse>): string | undefined {
    let code = getStringEnumKeyByValue(MessageCodeType, MessageCodeType.UNKNOWN_ERROR);

    if (isGenericErrorResponse(response.error)) {
      code = 'GENERIC_ERROR_CONFLICT';
    } else if (isValidationErrorResponse(response.error)) {
      code = 'VALIDATION_ERROR';
    } else if (isServerErrorResponse(response.error)) {
      code = getStringEnumKeyByValue(MessageCodeType, response.code);
    } else if (isEmptyErrorResponse(response.error)) {
      code = getStringEnumKeyByValue(MessageCodeType, response.code);
    }

    this.showErrorUI(code, response.error);
    return code;
  }

  public getValidationErrors(response: OperationResult<any, ApiDataErrorResponse>): { [field: string]: string[] } {
    if (isValidationErrorResponse(response.error)) {
      const errors: { [field: string]: string[] } = {};
      response.error.forEach(errorItem => {
        errors[errorItem.field] = errorItem.messages;
      });
      return errors;
    }
    return {};
  }

  private showErrorUI(code: string | undefined, details: any): void {
    // - console.error (debug)
    // - modal
    // - snackbar
    // - toast notification

    if (!code) {
      console.warn("ERROR UNDEFINED CODE");
    }

    console.error(`Error [${code}]`, details);



    // Ejemplo: si después integras Angular Material
    // this.snackBar.open(`Error: ${code}`, 'Cerrar', { duration: 5000 });

    // Ejemplo: si integras un modal service propio
    // this.modalService.openErrorDialog({ code, details });
  }


}
