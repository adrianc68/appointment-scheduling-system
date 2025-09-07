import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { AvailabilityTimeSlot } from '../../../view-model/business-entities/availability-time-slot';
import { SchedulerService } from '../../../model/communication-components/scheduler.service';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { map, Observable, of, switchMap } from 'rxjs';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';
import { Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { AvailabilityTimeSlotStatusType } from '../../../view-model/business-entities/types/availability-time-slot-status.types';
import { SlotDateRangePipe } from '../../../cross-cutting/helper/date-utils/slot-date-range.pipe';
import { ReadableTimePipe } from '../../../cross-cutting/helper/date-utils/readable-time.pipe';
import { FormsModule } from '@angular/forms';
import { ErrorUIService } from '../../../cross-cutting/communication/handle-error-service/error-ui.service';

@Component({
  selector: 'app-availability-time-slot-management',
  imports: [CommonModule, ...SHARED_STANDALONE_COMPONENTS, MatIconModule, SlotDateRangePipe, ReadableTimePipe, FormsModule],
  standalone: true,
  templateUrl: './availability-time-slot-management.component.html',
  styleUrl: './availability-time-slot-management.component.scss'
})
export class AvailabilityTimeSlotManagementComponent {
  systemMessage?: string = '';
  translationCodes = TranslationCodes;
  searchTerm: string = '';
  slots: AvailabilityTimeSlot[] = [];
  filteredItems: AvailabilityTimeSlot[] = [];
  selectedDate: string = new Date().toISOString().split("T")[0];
  startDate: string = this.selectedDate;
  endDate: string = this.selectedDate;
  openedSlots = new Set<number>();

  constructor(private schedulerService: SchedulerService, private errorUIService: ErrorUIService, private i18nService: I18nService, private router: Router) {
    this.getAvailabilityTimeSlots(this.startDate, this.endDate);
  }

  getAvailabilityTimeSlots(startDate: string, endDate: string): void {
    this.schedulerService.getAvailabilityTimeSlots(startDate, endDate).pipe(
      map((response: OperationResult<AvailabilityTimeSlot[], ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.slots = [...response.result!].sort((a, b) => {
            return new Date(b.startDate).getTime() - new Date(a.startDate).getTime();
          });
          this.filteredItems = this.slots;
          this.systemMessage = code;
          return of(true);
        } else {
          let code = this.errorUIService.handleError(response);
          this.systemMessage = code;
          return of(false);
        }
      })
    ).subscribe();
  }

  enableAvailabilityTimeSlot(uuid: string) {
    this.schedulerService.enableAvailabilityTimeSlot(uuid).pipe(
      map((response: OperationResult<boolean, ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          const index = this.slots.findIndex(slot => slot.uuid === uuid);
          if (index !== -1) {
            this.slots[index].status = AvailabilityTimeSlotStatusType.ENABLED;
          }
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(true);
        } else {
          let code = this.errorUIService.handleError(response);
          this.systemMessage = code;
          return of(false);
        }
      })
    ).subscribe();
  }

  disableAvailabilityTimeSlot(uuid: string) {
    this.schedulerService.disableAvailabilityTimeSlot(uuid).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          const index = this.slots.findIndex(slot => slot.uuid === uuid);
          if (index !== -1) {
            this.slots[index].status = AvailabilityTimeSlotStatusType.DISABLED;
          }
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(true);
        } else {
          let code = this.errorUIService.handleError(response);
          this.systemMessage = code;
          return of(false);
        }
      })
    ).subscribe();
  }

  deleteAvailabilityTimeSlot(uuid: string) {
    this.schedulerService.deleteAvailabilityTimeSlot(uuid).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          const index = this.slots.findIndex(slot => slot.uuid === uuid);
          if (index !== -1) {
            this.slots.splice(index, 1);
          }
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(true);
        } else {
          let code = this.errorUIService.handleError(response);
          this.systemMessage = code;
          return of(false);
        }
      })
    ).subscribe();
  }

  onDateRangeChange(value: string, type: 'start' | 'end') {
    if (type === 'start') {
      this.startDate = value;
    } else {
      this.endDate = value;
    }
    this.getAvailabilityTimeSlots(this.startDate, this.endDate)
  }

  applyFilters() {
    let tempItems = [...this.slots];

    if (this.searchTerm.trim()) {
      tempItems = tempItems.filter(item =>
        Object.values(item).some(value =>
          value?.toString().toLowerCase().includes(this.searchTerm.toLowerCase())
        )
      );
    }
    this.filteredItems = tempItems;
  }

  toggleSlot(index: number) {
    if (this.openedSlots.has(index)) {
      this.openedSlots.delete(index);
    } else {
      this.openedSlots.add(index);
    }
  }

  redirectToRegisterSlot() {
    this.router.navigate([WebRoutes.availability_time_slot_management_register_slot])
  }

  redirectToEditSlot(slot: AvailabilityTimeSlot) {
    this.router.navigate([WebRoutes.availability_time_slot_management_edit_slot], { state: { slot } });
  }


  translate(key: string): string {
    return this.i18nService.translate(key);
  }



}
