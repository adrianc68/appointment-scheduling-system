import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'slotDateRange'
})
export class SlotDateRangePipe implements PipeTransform {


  transform(start: string | Date, end: string | Date): string {
    if (!start || !end) return '';

    const startStr = start instanceof Date ? start.toLocaleString() : start;
    const endStr = end instanceof Date ? end.toLocaleString() : end;

    const [startDate, startTime] = startStr.split(', ');
    const [endDate, endTime] = endStr.split(', ');

    const datePart = startDate === endDate ? startDate : `${startDate} - ${endDate}`;

    return `${datePart} | ${startTime} - ${endTime}`;
  }
}
