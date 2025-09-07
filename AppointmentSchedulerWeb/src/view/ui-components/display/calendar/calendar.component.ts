import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FullCalendarComponent, FullCalendarModule } from '@fullcalendar/angular';
import { CalendarOptions, DatesSetArg } from '@fullcalendar/core';

import dayGridPlugin from '@fullcalendar/daygrid';      // vista mes
import timeGridPlugin from '@fullcalendar/timegrid';    // vista semana/día
import interactionPlugin from '@fullcalendar/interaction'; // interacciones (drag/drop)




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

  calendarOptions: CalendarOptions = {
    initialView: 'dayGridMonth',
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,timeGridWeek,timeGridDay'
    },
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
    datesSet: this.handleDatesSet.bind(this),
    dateClick: this.handleDateClick.bind(this),
    events: []
  };

  handleDateClick(arg: any) {
    this.dateSelected.emit(arg.dateStr);
  }

  handleDatesSet(arg: DatesSetArg) {
    this.currentDateChange.emit(arg.view.currentStart);
  }

  ngOnChanges() {
    if (this.slots && this.slots.length > 0 && this.calendar) {
      const calendarApi = this.calendar.getApi();
      calendarApi.removeAllEvents(); // limpia eventos antiguos
      this.slots.forEach(slot => {
        calendarApi.addEvent({
          start: slot.startDate,
          end: slot.endDate,
          allDay: true,
          display: 'background',
          backgroundColor: '#4caf50',
          //title: 'Disponible'
        });
      });
    }
  }
}
