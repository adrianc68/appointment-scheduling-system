import { Injectable } from '@angular/core';
import { MessageCodeType } from './message-code.types';
import { OperationResult } from './operation-result';

@Injectable({
  providedIn: 'root'
})
export class OperationResultService {

  constructor() { }

  createSuccess<T>(result: T, code: MessageCodeType = MessageCodeType.OK): OperationResult<T, any> {
    return {
      isSuccessful: true,
      code: code,
      result: result
    };
  }

  createFailure<TError>(error: TError, code: MessageCodeType = MessageCodeType.ERROR): OperationResult<any, TError> {
    return {
      isSuccessful: false,
      code: code,
      error: error
    };
  }

  createFailureList<TError>(errors: TError[], code: MessageCodeType = MessageCodeType.ERROR): OperationResult<any, TError> {
    return {
      isSuccessful: false,
      code: code,
      errors: errors
    };
  }
}
