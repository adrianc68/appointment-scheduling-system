export function formatReadableDate(isoString: string): string {
  const date = new Date(isoString);

  const formattedDate = date.toLocaleDateString('es-MX', {
    day: '2-digit',
    month: 'long',
    year: 'numeric',
  });

  const formattedTime = date.toLocaleTimeString('es-MX', {
    hour: '2-digit',
    minute: '2-digit',
    hour12: true,
  });

  return `${formattedDate}, ${formattedTime}`;
}

/**
 * Converts a local Date object to a UTC Date object.
 * @param localDate Local Date object
 * @returns UTC Date object
 */
export function fromLocalToUTC(value: string | Date): string {
  if (!value) throw new Error("Invalid date value");

  let localDate: Date;
  if (typeof value === "string") {
    const [year, month, day, hour, minute, second] = value.match(/\d+/g)!.map(Number);
    localDate = new Date(year, month - 1, day, hour, minute, second || 0);
  } else {
    localDate = value;
  }

  // Convertimos a UTC “fijando la misma hora”
  const utcDate = new Date(
    localDate.getFullYear(),
    localDate.getMonth(),
    localDate.getDate(),
    localDate.getHours(),
    localDate.getMinutes(),
    localDate.getSeconds()
  );

  return utcDate.toISOString();
}

/**
 * Convierte un ISO string UTC a un string en hora local.
 * @param value ISO string UTC o Date
 * @returns string en formato "YYYY-MM-DDTHH:mm:ss" local
 */
export function fromUTCtoLocal(value: string | Date): string {
  let date: Date;

  if (typeof value === "string") {
    date = new Date(value); // JS interpreta como UTC
  } else {
    date = value;
  }

  // Obtener componentes locales
  const year = date.getFullYear();
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const day = date.getDate().toString().padStart(2, '0');
  const hours = date.getHours().toString().padStart(2, '0');
  const minutes = date.getMinutes().toString().padStart(2, '0');
  const seconds = date.getSeconds().toString().padStart(2, '0');

  return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`;
}


export function fromLocalDateTimeToUTC(dateTimeLocal: string): Date {
  // dateTimeLocal example: "2025-08-30T09:00"
  const [date, time] = dateTimeLocal.split('T');
  const [year, month, day] = date.split('-').map(Number);
  const [hours, minutes] = time.split(':').map(Number);

  // Month is 0-indexed in JS Date
  return new Date(Date.UTC(year, month - 1, day, hours, minutes));
}
