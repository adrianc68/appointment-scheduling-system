import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { SlotDateRangePipe } from '../../../../../cross-cutting/helper/date-utils/slot-date-range.pipe';
import { ReadableTimePipe } from '../../../../../cross-cutting/helper/date-utils/readable-time.pipe';
import { ReadableDatePipe } from '../../../../../cross-cutting/helper/date-utils/readable-date.pipe';
import { DurationDatePipe } from '../../../../../cross-cutting/helper/date-utils/duration-date.pipe';
import { CalendarComponent } from '../../../../ui-components/display/calendar/calendar.component';
import { AuthenticationService } from '../../../../../cross-cutting/security/authentication/authentication.service';
import { AccountData } from '../../../../../view-model/business-entities/account';
import { Subscription, tap } from 'rxjs';
import { SchedulerService } from '../../../../../model/communication-components/scheduler.service';
import { Appointment } from '../../../../../view-model/business-entities/appointment';
import { OperationResult } from '../../../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse } from '../../../../../cross-cutting/communication/model/api-response.error';
import { MessageCodeType } from '../../../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../../../cross-cutting/helper/enum-utils/enum.utils';
import { ErrorUIService } from '../../../../../cross-cutting/communication/handle-error-service/error-ui.service';
import { AppointmentStatusType } from '../../../../../view-model/business-entities/types/appointment-status.types';




@Component({
  selector: 'app-client-home',
  imports: [CommonModule, FormsModule, MatIconModule, SlotDateRangePipe, ReadableTimePipe, ReadableDatePipe, DurationDatePipe, CalendarComponent, ReadableDatePipe, ReadableTimePipe],
  standalone: true,
  templateUrl: './client-home.component.html',
  styleUrl: './client-home.component.scss'
})
export class ClientHomeComponent implements OnInit, OnDestroy {
  accountData: AccountData | null = null;
  private subscription: Subscription | null = null;
  selectedDate: string = new Date().toISOString().split("T")[0];
  startDate: string = this.selectedDate;
  endDate: string = this.selectedDate;
  today: Date = new Date();

  systemMessage?: string = '';

  scheduledAppointments: Appointment[] = [];
  upcomingAppointments: Appointment[] = [];
  pastAppointments: Appointment[] = [];
  allAppointments: Appointment[] = [];

  constructor(private authenticationService: AuthenticationService, private schedulerService: SchedulerService, private errorUIService: ErrorUIService) {
    const todayISO = this.today.toISOString().split("T")[0];

    this.startDate = todayISO;

    const future = new Date(this.today);
    future.setMonth(this.today.getMonth() + 1);
    this.endDate = future.toISOString().split("T")[0];

    this.loadAllAppointments();

  }

  ngOnInit(): void {
    this.subscription = this.authenticationService.getAccountData()
      .subscribe(data => {
        this.accountData = data;
      });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  errorValidationMessage: { [field: string]: string[] } = {};

  private setErrorValidationMessage(key: string, value: string[]) {
    this.errorValidationMessage[key.toLowerCase()] = value;
  }


  loadAllAppointments(): void {
    this.allAppointments = [];
    this.upcomingAppointments = [];
    this.pastAppointments = [];

    this.schedulerService.getAllAppointmentsOfClient().pipe(
      tap((response: OperationResult<Appointment[], ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          const code = getStringEnumKeyByValue(MessageCodeType, response.code);

          this.allAppointments = [...response.result!].sort(
            (a, b) => new Date(a.startDate).getTime() - new Date(b.startDate).getTime()
          );

          const now = new Date();

          this.upcomingAppointments = this.allAppointments.filter(
            app => new Date(app.startDate) >= now && app.status !== AppointmentStatusType.FINISHED && app.status !== AppointmentStatusType.CANCELED

          );

          this.pastAppointments = this.allAppointments
            .filter(app => new Date(app.startDate) < now || app.status === AppointmentStatusType.FINISHED)
            .sort((a, b) => new Date(b.startDate).getTime() - new Date(a.startDate).getTime())
            .map(app => ({ ...app, status: AppointmentStatusType.FINISHED }));

          this.systemMessage = code;
        } else {
          const code = this.errorUIService.handleError(response);
          this.systemMessage = code;

          const validationErrors = this.errorUIService.getValidationErrors(response);
          Object.entries(validationErrors).forEach(([field, messages]) => {
            this.setErrorValidationMessage(field, messages);
          });
        }
      })
    ).subscribe();
  }





}


