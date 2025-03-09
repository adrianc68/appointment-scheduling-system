export class InvalidValueEnumValueException extends Error {
  constructor(message: string, public readonly details?: any) {
    super(message);
    this.name = "InvalidEnumValueException";
    Object.setPrototypeOf(this, InvalidValueEnumValueException.prototype);
  }
}
