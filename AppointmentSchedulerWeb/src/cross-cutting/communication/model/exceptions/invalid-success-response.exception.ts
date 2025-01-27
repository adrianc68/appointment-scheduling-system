export class InvalidSuccessResponseError extends Error {
  constructor(message: string, public readonly details?: any) {
    super(message);
    this.name = "InvalidSuccessResponseError";
    Object.setPrototypeOf(this, InvalidSuccessResponseError.prototype);
  }
}
