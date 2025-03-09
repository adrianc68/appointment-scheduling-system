export class InvalidApiDataErrorResponseError extends Error {
  constructor(message: string, public readonly details?: any) {
    super(message);
    this.name = "InvalidApiDataErrorResponseError";
    Object.setPrototypeOf(this, InvalidApiDataErrorResponseError.prototype);
  }
}
