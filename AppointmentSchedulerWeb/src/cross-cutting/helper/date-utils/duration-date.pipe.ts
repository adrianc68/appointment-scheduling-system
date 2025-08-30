import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'durationDate'
})
export class DurationDatePipe implements PipeTransform {

  transform(startDate: Date, endDate: Date): string {
    if (!startDate || !endDate) return '';

    const diffMs = endDate.getTime() - startDate.getTime();
    const diffMin = Math.floor(diffMs / (1000 * 60));
    const hours = Math.floor(diffMin / 60);
    const minutes = diffMin % 60;

    return hours > 0 ? `${hours} h ${minutes} min` : `${minutes} min`;
  }

}
