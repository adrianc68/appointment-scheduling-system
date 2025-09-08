import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FullCalendarComponent, FullCalendarModule } from '@fullcalendar/angular';
import { CalendarOptions, DatesSetArg } from '@fullcalendar/core';

import dayGridPlugin from '@fullcalendar/daygrid';      // vista mes
import timeGridPlugin from '@fullcalendar/timegrid';    // vista semana/día
import interactionPlugin from '@fullcalendar/interaction'; // interacciones (drag/drop)
import esLocale from '@fullcalendar/core/locales/es';




@Component({
  selector: 'app-calendar',
  imports: [FullCalendarModule],
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.scss'
})
export class CalendarComponent {
  @ViewChild('calendar') calendar!: FullCalendarComponent;

  @Output() dateSelected = new EventEmitter<string>();
  @Output() currentDateChange = new EventEmitter<Date>();
  @Input() slots: { startDate: string, endDate: string }[] = [];
  @Input() selectedDate: string | null = null;


  calendarOptions: CalendarOptions = {
    initialView: 'dayGridMonth',
    contentHeight: 'auto',
    locales: [esLocale],
    fixedWeekCount: false,

    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: ''
    },
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
    datesSet: this.handleDatesSet.bind(this),
    dateClick: this.handleDateClick.bind(this),
    events: []
  };

  handleDateClick(arg: any) {
    this.selectedDate = arg.dateStr;

    // Quitar selección previa
    document.querySelectorAll('.fc-daygrid-day').forEach(cell => {
      cell.classList.remove('day-selected');
    });

    // Agregar clase al día clicado
    arg.dayEl.classList.add('day-selected');

    this.dateSelected.emit(arg.dateStr);
  }

  handleDatesSet(arg: DatesSetArg) {
    this.currentDateChange.emit(arg.view.currentStart);

    if (this.selectedDate) {
      const dayCell = document.querySelector(`.fc-daygrid-day[data-date="${this.selectedDate}"]`);
      if (dayCell) {
        dayCell.classList.add('day-selected');
      }
    }
  }

  ngOnChanges() {
    if (this.slots && this.slots.length > 0 && this.calendar) {
      const calendarApi = this.calendar.getApi();
      calendarApi.removeAllEvents();

      const daysSet = new Set<string>();

      this.slots.forEach(slot => {
        const start = new Date(slot.startDate);
        const end = new Date(slot.endDate);

        let current = new Date(start);
        while (current <= end) {
          daysSet.add(current.toISOString().split('T')[0]);
          current.setDate(current.getDate() + 1);
        }
      });

      daysSet.forEach(day => {
        calendarApi.addEvent({
          start: day,
          allDay: true,
          display: 'background',
          backgroundColor: '#4caf50',
        });
      });
    }

    if (this.selectedDate) {
      const dayCell = document.querySelector(`.fc-daygrid-day[data-date="${this.selectedDate}"]`);
      if (dayCell) {
        dayCell.classList.add('day-selected');
      }
    }
  }
}
