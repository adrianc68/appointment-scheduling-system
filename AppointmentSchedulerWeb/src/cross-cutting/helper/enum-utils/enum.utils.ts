export function getStringEnumKeyByValue<T extends object>(enumObject: T, value: T[keyof T]): string | undefined {
  return Object.keys(enumObject).find((key) => enumObject[key as keyof T] === value);
}

export function isEnumValue<T>(enumObject: T, code: string, expected: T[keyof T]): boolean {
  return code === enumObject[expected as keyof T];
}

export function parseStringToEnum<T extends object>(enumObject: T, codeString: string): T[keyof T] | undefined {
  const enumValue = Object.keys(enumObject).find(key => key.toUpperCase() === codeString.toUpperCase());
  if (enumValue) {
    return enumObject[enumValue as keyof T];
  }
  return undefined;

}

