import { Component } from '@angular/core';
import { FullCalendarModule } from '@fullcalendar/angular';
import { CalendarOptions } from '@fullcalendar/core';

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
  calendarOptions: CalendarOptions = {
    initialView: 'dayGridMonth',
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,timeGridWeek,timeGridDay'
    },
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
    dateClick: this.handleDateClick.bind(this), // callback al hacer click en un día

    //events: [
    //  { title: 'Evento 1', date: '2025-09-10' },
    //  { title: 'Evento 2', date: '2025-09-15' }
    //]
  };


  handleDateClick(arg: any) {
    alert('Seleccionaste la fecha: ' + arg.dateStr);
    // aquí podrías abrir un modal, crear un evento, etc.
  }
}
