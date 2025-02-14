function formatReadableDate(isoString: string): string {
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

