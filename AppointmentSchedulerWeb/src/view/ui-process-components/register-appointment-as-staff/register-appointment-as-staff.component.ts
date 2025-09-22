import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { ServiceOffer } from '../../../view-model/business-entities/service-offer';
import { Appointment } from '../../../view-model/business-entities/appointment';
import { SchedulerService } from '../../../model/communication-components/scheduler.service';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { map } from 'rxjs';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { Client } from '../../../view-model/business-entities/client';
import { ClientService } from '../../../model/communication-components/client.service';
import { ReadableTimePipe } from '../../../cross-cutting/helper/date-utils/readable-time.pipe';
import { ReadableDatePipe } from '../../../cross-cutting/helper/date-utils/readable-date.pipe';
import { CalendarComponent } from '../../ui-components/display/calendar/calendar.component';
import { AvailabilityTimeSlot } from '../../../view-model/business-entities/availability-time-slot';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { ErrorUIService } from '../../../cross-cutting/communication/handle-error-service/error-ui.service';
import { DurationDatePipe } from '../../../cross-cutting/helper/date-utils/duration-date.pipe';

@Component({
  selector: 'app-register-appointment-as-staff',
  imports: [CommonModule, FormsModule, MatIconModule, ReadableTimePipe, ReadableDatePipe, CalendarComponent, DragDropModule],
  templateUrl: './register-appointment-as-staff.component.html',
  styleUrl: './register-appointment-as-staff.component.scss'
})
export class RegisterAppointmentAsStaffComponent {

  systemMessage?: string = '';
  translationCodes = TranslationCodes
  servicesAvailable: ServiceOffer[] = [];
  scheduledAppointments: Appointment[] = [];
  clients: Client[] = [];
  selectedDate: string = new Date().toISOString().split("T")[0];
  selectedServicesOffer: ServiceOffer[] = [];
  startTimes: { [uuid: string]: string } = {};
  selectedClient?: Client;
  slots: { startDate: string, endDate: string }[] = [];
  globalStartTime: string = '';

  availabilitySlots: AvailabilityTimeSlot[] = [];

  openedSlots = new Set<number>();
  currentStep = 1;
  previousStep = 1;

  ngAfterViewChecked(): void {
    if (this.previousStep !== this.currentStep) {
      window.scrollTo(0, 0);
      this.previousStep = this.currentStep;
    }
  }

  constructor(private schedulerService: SchedulerService, private i18nService: I18nService, private clientService: ClientService, private errorUIService: ErrorUIService) {
    this.getClientList();
    this.onDateSelected(this.selectedDate);
  }

  getClientList(): void {
    this.clientService.getClientList().pipe(
      map((response: OperationResult<Client[], ApiDataErrorResponse>): boolean => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          this.clients = [...response.result!];
          return true;
        } else {
          this.errorUIService.handleError(response);
          const validationErrors = this.errorUIService.getValidationErrors(response);
          Object.entries(validationErrors).forEach(([field, messages]) => {
            this.setErrorValidationMessage(field, messages);
          });
          return false;
        }
      })
    ).subscribe();
  }

  blockTimeRange(): void {
    const payload = {
      date: this.selectedDate,
      clientUuid: this.selectedClient?.uuid,
      selectedServices: this.selectedServicesOffer.map(s => ({
        uuid: s.uuid,
        startTime: this.startTimes[s.uuid]
      }))
    };

    this.schedulerService.blockTimeRange(payload).pipe(
      map((response: OperationResult<Date, ApiDataErrorResponse>): boolean => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = response.result ? new Date(response.result).toISOString() : code;
          return true;
        } else {
          let code = this.errorUIService.handleError(response);
          this.systemMessage = code;
          const validationErrors = this.errorUIService.getValidationErrors(response);
          Object.entries(validationErrors).forEach(([field, messages]) => {
            this.setErrorValidationMessage(field, messages);
          });
          return false;
        }
      })
    ).subscribe();
  }


  registerAppointmentAsStaff(): void {

    const payload = {
      date: this.selectedDate,
      clientUuid: this.selectedClient?.uuid,
      selectedServices: this.selectedServicesOffer.map(s => ({
        uuid: s.uuid,
        startTime: this.startTimes[s.uuid]
      }))
    };

    this.schedulerService.registerAppointmentAsStaff(payload).pipe(
      map((response: OperationResult<Date, ApiDataErrorResponse>): boolean => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return true;
        } else {
          let code = this.errorUIService.handleError(response);
          this.systemMessage = code;
          const validationErrors = this.errorUIService.getValidationErrors(response);
          Object.entries(validationErrors).forEach(([field, messages]) => {
            this.setErrorValidationMessage(field, messages);
          });

          return false;
        }
      })
    ).subscribe();
  }



  onDateSelected(date: string) {
    this.selectedDate = date;
    this.loadAppointments(date, date);
    this.getAvailableServices(date);
  }

  onCurrentDateChange(date: Date) {
    const start = new Date(date.getFullYear(), date.getMonth(), 1);
    const end = new Date(date.getFullYear(), date.getMonth() + 1, 0);

    this.getAvailabilityTimeSlots(
      this.formatDateToApi(start),
      this.formatDateToApi(end)
    );

    this.selectedServicesOffer = [];
    this.selectedClient = undefined;
  }

  private formatDateToApi(date: Date): string {
    const year = date.getUTCFullYear();
    const month = date.getUTCMonth() + 1;
    const day = date.getUTCDate();
    return `${year}-${month}-${day}`;
  }


  errorValidationMessage: { [field: string]: string[] } = {};

  loadAppointments(startDate: string, endDate: string): void {
    this.scheduledAppointments = [];

    this.schedulerService.getScheduledOrConfirmedAppointmentsAsStaff(startDate, endDate).pipe(
      map((response: OperationResult<Appointment[], ApiDataErrorResponse>): boolean => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          this.scheduledAppointments = [...response.result!];
          return true;
        } else {
          this.errorUIService.handleError(response);
          const validationErrors = this.errorUIService.getValidationErrors(response);
          Object.entries(validationErrors).forEach(([field, messages]) => {
            this.setErrorValidationMessage(field, messages);
          });
          return false;
        }
      })
    ).subscribe();
  }

  getAvailableServices(date: string) {
    this.schedulerService.getAvailableServices(date).pipe(
      map((response: OperationResult<ServiceOffer[], ApiDataErrorResponse>): boolean => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          this.servicesAvailable = [...response.result!].sort((a, b) =>
            a.name.localeCompare(b.name)
          );
          return true;
        } else {
          this.errorUIService.handleError(response);
          const validationErrors = this.errorUIService.getValidationErrors(response);
          Object.entries(validationErrors).forEach(([field, messages]) => {
            this.setErrorValidationMessage(field, messages);
          });
          return false;
        }
      })
    ).subscribe();
  }

  getAvailabilityTimeSlots(startDate: string, endDate: string) {
    this.schedulerService.getAvailabilityTimeSlots(startDate, endDate).pipe(
      map((response: OperationResult<AvailabilityTimeSlot[], ApiDataErrorResponse>): boolean => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          this.availabilitySlots = response.result ?? [];
          this.slots = response.result!.map(slot => ({
            startDate: new Date(slot.startDate).toISOString(),
            endDate: new Date(slot.endDate).toISOString()
          }));
          return true;
        } else {
          this.errorUIService.handleError(response);
          const validationErrors = this.errorUIService.getValidationErrors(response);
          Object.entries(validationErrors).forEach(([field, messages]) => {
            this.setErrorValidationMessage(field, messages);
          });
          // return false;
          return false;
        }
      })
    ).subscribe();
  }


  private setErrorValidationMessage(key: string, value: string[]) {
    this.errorValidationMessage[key.toLowerCase()] = value;
  }


  selectServiceOffer(serviceOffer: ServiceOffer) {
    const exists = this.selectedServicesOffer.some(s => s.uuid === serviceOffer.uuid);
    if (!exists) {
      this.selectedServicesOffer.push(serviceOffer);

      this.updateStartTimesFromGlobal();

      const index = this.servicesAvailable.findIndex(service => service.uuid === serviceOffer.uuid);
      if (index !== -1) {
        this.servicesAvailable.splice(index, 1);
      }
    }
  }

  removeSelectedService(serviceOffer: ServiceOffer) {
    const index = this.selectedServicesOffer.findIndex(service => service.uuid === serviceOffer.uuid);
    if (index !== -1) {
      this.selectedServicesOffer.splice(index, 1);

      this.servicesAvailable.push(serviceOffer);
    }
  }








  dropService(event: CdkDragDrop<any[]>) {
    moveItemInArray(this.selectedServicesOffer, event.previousIndex, event.currentIndex);
    this.updateStartTimesFromGlobal();
  }

  updateStartTimesFromGlobal() {
    if (!this.globalStartTime) return;

    let [h, m, s] = this.globalStartTime.split(':').map(Number);
    let currentDate = new Date();
    currentDate.setHours(h, m, 0, 0);

    this.selectedServicesOffer.forEach(service => {
      const startStr = currentDate.toTimeString().split(' ')[0];
      this.startTimes[service.uuid] = startStr;

      currentDate.setMinutes(currentDate.getMinutes() + service.minutes);
    });
  }

  getEndTime(service: ServiceOffer): string {
    const start = this.startTimes[service.uuid];
    if (!start) return '';
    const [hours, minutes, seconds] = start.split(':').map(Number);
    const totalMinutes = hours * 60 + minutes + service.minutes;
    const endHours = Math.floor(totalMinutes / 60);
    const endMinutes = totalMinutes % 60;
    return `${String(endHours).padStart(2, '0')}:${String(endMinutes).padStart(2, '0')}:00`;
  }

  getEarliestStartTime(): Date | null {
    if (!this.selectedServicesOffer || this.selectedServicesOffer.length === 0) return null;

    const times = this.selectedServicesOffer
      .map(s => this.startTimes[s.uuid])
      .filter(t => t)
      .map(t => new Date(`${this.selectedDate}T${t}`));

    if (times.length === 0) return null;

    return new Date(Math.min(...times.map(d => d.getTime())));
  }

  getEstimatedEndTime(): Date | null | undefined {
    const start = this.getEarliestStartTime();
    if (!start) return null;

    const totalMinutes = this.getTotalMinutes();
    const end = new Date(start.getTime());
    end.setMinutes(end.getMinutes() + totalMinutes);

    return end;
  }

  allStartTimesDefined(): boolean {
    if (!this.selectedServicesOffer || this.selectedServicesOffer.length === 0) return false;

    return this.selectedServicesOffer.every(service => {
      const startTime = this.startTimes[service.uuid];
      return startTime && startTime.trim() !== '';
    });
  }




  private hasTimeConflict(start1: Date, end1: Date, start2: Date, end2: Date): boolean {
    return start1 < end2 && end1 > start2;
  }


  getServiceConflictInfo(service: ServiceOffer): ConflictInfo {
    const startStr = this.startTimes[service.uuid];
    if (!startStr) return { hasConflict: false, conflictingAppointments: [], unavailable: false };

    const [hours, minutes] = startStr.split(':').map(Number);
    const start = new Date(this.selectedDate + 'T00:00:00');
    start.setHours(hours, minutes, 0, 0);

    const end = new Date(start);
    end.setMinutes(end.getMinutes() + service.minutes);

    const conflictingAppointments = this.scheduledAppointments.flatMap(app => {
      if (!app.scheduledServices) return [];

      return app.scheduledServices
        .filter(s => s.assistant.uuid === service.assistant?.uuid)
        .filter(s => this.hasTimeConflict(start, end, new Date(s.service.startDate), new Date(s.service.endDate)))
        .map(s => ({
          start: new Date(s.service.startDate),
          end: new Date(s.service.endDate),
          clientName: app.client?.name || 'Cliente',
          serviceName: s.service?.name,
        }));
    });

    const isAvailable = service.assistant ? this.isAssistantAvailable(service.assistant.uuid, start, end) : true;

    return {
      hasConflict: conflictingAppointments.length > 0 || !isAvailable,
      conflictingAppointments,
      unavailable: !isAvailable
    };
  }






  isServiceInConflict(service: ServiceOffer): boolean {
    const info = this.getServiceConflictInfo(service);
    return info.hasConflict;
  }

  isAssistantAvailable(assistantUuid: string, start: Date, end: Date): boolean {
    const { dayStart, dayEnd } = this.getDayBoundsFromSelectedDate();

    const assistantSlots = this.availabilitySlots.filter(
      slot =>
        slot.assistant.uuid === assistantUuid &&
        new Date(slot.endDate).getTime() >= dayStart.getTime() &&
        new Date(slot.startDate).getTime() <= dayEnd.getTime()
    );

    if (!assistantSlots.length) return false;

    return assistantSlots.some(slot => {
      const slotStart = new Date(slot.startDate);
      const slotEnd = new Date(slot.endDate);

      if (start.getTime() >= slotStart.getTime() && end.getTime() <= slotEnd.getTime()) {
        const conflictWithUnavailable = (slot.unavailableTimeSlots || []).some(u =>
          start.getTime() < new Date(u.endDate).getTime() &&
          end.getTime() > new Date(u.startDate).getTime()
        );
        return !conflictWithUnavailable;
      }
      return false;
    });
  }

  getUnavailableRangesForService(service: ServiceOffer) {
    if (!service.assistant) return [];

    const conflicts: { start: Date; end: Date }[] = [];

    this.availabilitySlots.forEach(slot => {
      if (slot.assistant?.uuid !== service.assistant!.uuid) return;

      const serviceStartStr = this.startTimes[service.uuid];
      if (!serviceStartStr) return;

      const [hours, minutes] = serviceStartStr.split(':').map(Number);
      const serviceStart = new Date(this.selectedDate + 'T00:00:00');
      serviceStart.setHours(hours, minutes, 0, 0);
      const serviceEnd = new Date(serviceStart);
      serviceEnd.setMinutes(serviceEnd.getMinutes() + service.minutes);

      const overlaps = slot.unavailableTimeSlots.some(u =>
        this.hasTimeConflict(serviceStart, serviceEnd, new Date(u.startDate), new Date(u.endDate))
      );

      if (overlaps) {
        slot.unavailableTimeSlots.forEach(u => {
          conflicts.push({ start: new Date(u.startDate), end: new Date(u.endDate) });
        });
      }
    });

    return conflicts;
  }


  getAvailabilityAndUnavailableForService(service: ServiceOffer) {
    if (!service.assistant) return [];

    const selectedDate = new Date(this.selectedDate);
    const dayStart = new Date(selectedDate);
    dayStart.setHours(0, 0, 0, 0);
    const dayEnd = new Date(selectedDate);
    dayEnd.setHours(23, 59, 59, 999);

    const ranges: { start: Date; end: Date; type: 'availability' | 'unavailable' }[] = [];

    this.availabilitySlots.forEach(slot => {
      if (slot.assistant?.uuid !== service.assistant!.uuid) return;

      const slotStart = new Date(slot.startDate);
      const slotEnd = new Date(slot.endDate);

      if (slotEnd > dayStart && slotStart < dayEnd) {
        ranges.push({ start: slotStart, end: slotEnd, type: 'availability' });

        slot.unavailableTimeSlots.forEach(u => {
          const uStart = new Date(u.startDate);
          const uEnd = new Date(u.endDate);
          if (uEnd > dayStart && uStart < dayEnd) {
            ranges.push({ start: uStart, end: uEnd, type: 'unavailable' });
          }
        });
      }
    });

    return ranges;
  }



  getServicesWithConflictInfo() {
    return this.selectedServicesOffer.map(service => ({
      service,
      conflictInfo: this.getServiceConflictInfo(service)
    }));
  }


  private getDayBoundsFromSelectedDate(): { dayStart: Date; dayEnd: Date } {
    const [y, m, d] = this.selectedDate.split('-').map(Number);
    const dayStart = new Date(y, m - 1, d, 0, 0, 0, 0);
    const dayEnd = new Date(y, m - 1, d, 23, 59, 59, 999);
    return { dayStart, dayEnd };
  }

  getAvailabilityAndUnavailableForSelectedDate(service: ServiceOffer) {
    if (!service.assistant) return [];

    const { dayStart, dayEnd } = this.getDayBoundsFromSelectedDate();

    return this.availabilitySlots
      .filter(slot =>
        slot.assistant?.uuid === service.assistant!.uuid &&
        new Date(slot.endDate) >= dayStart &&
        new Date(slot.startDate) <= dayEnd
      )
      .map(slot => ({
        availabilityStart: new Date(slot.startDate),
        availabilityEnd: new Date(slot.endDate),
        unavailableSlots: (slot.unavailableTimeSlots || [])
          .filter(u => new Date(u.endDate) >= dayStart && new Date(u.startDate) <= dayEnd)
          .map(u => ({ start: new Date(u.startDate), end: new Date(u.endDate) }))
      }))
      .sort((a, b) => a.availabilityStart.getTime() - b.availabilityStart.getTime());
  }


  getTotalMinutes(): number {

    if (!this.selectedServicesOffer) return 0;
    return this.selectedServicesOffer.reduce((total, service) => total + service.minutes, 0);
  }

  getTotalPrice(): number {
    if (!this.selectedServicesOffer) return 0;
    return this.selectedServicesOffer.reduce((total, service) => total + service.price, 0);
  }


  nextStep() {
    this.currentStep++;
  }

  prevStep() {
    this.currentStep--;
  }


  trackByUuid(_: number, service: ServiceOffer) {
    return service.uuid;
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }



}



interface ConflictInfo {
  hasConflict: boolean;
  conflictingAppointments: { start: Date; end: Date; clientName: string; serviceName?: string }[];
  unavailable: boolean; // nuevo
}
