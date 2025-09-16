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

export function fromLocalToUTC(value: string | Date): string {
  if (!value) throw new Error("Invalid date value");

  let localDate: Date;
  if (typeof value === "string") {
    localDate = new Date(value);
  } else {
    localDate = value;
  }

  return localDate.toISOString();
}


export function fromUTCtoLocal(value: string | Date): string {
  let date: Date;

  if (typeof value === "string") {
    date = new Date(value);
  } else {
    date = value;
  }

  const year = date.getFullYear();
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const day = date.getDate().toString().padStart(2, '0');
  const hours = date.getHours().toString().padStart(2, '0');
  const minutes = date.getMinutes().toString().padStart(2, '0');
  const seconds = date.getSeconds().toString().padStart(2, '0');

  return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`;
}


export function fromLocalDateTimeToUTC(dateTimeLocal: string): Date {
  const [date, time] = dateTimeLocal.split('T');
  const [year, month, day] = date.split('-').map(Number);
  const [hours, minutes] = time.split(':').map(Number);

  return new Date(Date.UTC(year, month - 1, day, hours, minutes));
}

