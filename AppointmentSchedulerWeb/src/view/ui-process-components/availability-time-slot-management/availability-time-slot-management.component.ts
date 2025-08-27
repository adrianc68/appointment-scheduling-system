import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { AvailabilityTimeSlot } from '../../../view-model/business-entities/availability-time-slot';
import { SchedulerService } from '../../../model/communication-components/scheduler.service';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { Observable, of, switchMap } from 'rxjs';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { ApiDataErrorResponse, isEmptyErrorResponse, isGenericErrorResponse, isServerErrorResponse, isValidationErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';
import { Router } from '@angular/router';
import { ReadableDatePipe } from '../../../cross-cutting/helper/date-utils/readable-date.pipe';
import { MatIconModule } from '@angular/material/icon';
import { AvailabilityTimeSlotStatusType } from '../../../view-model/business-entities/types/availability-time-slot-status.types';
import { SlotDateRangePipe } from '../../../cross-cutting/helper/date-utils/slot-date-range.pipe';
import { ReadableTimePipe } from '../../../cross-cutting/helper/date-utils/readable-time.pipe';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-availability-time-slot-management',
  imports: [CommonModule, ...SHARED_STANDALONE_COMPONENTS, ReadableDatePipe, MatIconModule, SlotDateRangePipe, ReadableTimePipe, FormsModule],
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
  //setFilters: boolean = false;
  //activeFilters: { [key: string]: any } = {};
  //filters: { key: string, label: string, type: 'boolean' | 'enum' | 'date' | 'datetime' | 'multi', enumValues?: any[] }[] = [];

  applyFilters() {
    let tempItems = [...this.slots];

    if (this.searchTerm.trim()) {
      tempItems = tempItems.filter(item =>
        Object.values(item).some(value =>
          value?.toString().toLowerCase().includes(this.searchTerm.toLowerCase())
        )
      );
    }
    //
    //if (this.setFilters) {
    //  Object.keys(this.activeFilters).forEach(filterKey => {
    //    const filterValue = this.activeFilters[filterKey];
    //    if (filterValue !== undefined && filterValue !== null && filterValue !== '') {
    //      tempItems = tempItems.filter(item => {
    //        const itemValue = item[filterKey];
    //        const filterType = this.filters.find(f => f.key === filterKey)?.type;
    //        switch (filterType) {
    //          case 'boolean':
    //            return itemValue === filterValue;
    //          case 'enum':
    //            return Array.isArray(filterValue) ? filterValue.includes(itemValue) : itemValue === filterValue;
    //          case 'date':
    //            return new Date(itemValue).toISOString().split('T')[0] ===
    //              new Date(filterValue).toISOString().split('T')[0];
    //          case 'datetime':
    //            return new Date(itemValue).toISOString().slice(0, 16) ===
    //              new Date(filterValue).toISOString().slice(0, 16);
    //          default:
    //            return true;
    //        }
    //      });
    //    }
    //  });
    //}
    this.filteredItems = tempItems;
  }


  openedSlots = new Set<number>();

  toggleSlot(index: number) {
    if (this.openedSlots.has(index)) {
      this.openedSlots.delete(index);
    } else {
      this.openedSlots.add(index);
    }
  }



  constructor(private schedulerService: SchedulerService, private i18nService: I18nService, private logginService: LoggingService, private router: Router) {
    this.schedulerService.getAvailabilityTimeSlots("2024-1-11", "2026-1-11").pipe(
      switchMap((response: OperationResult<AvailabilityTimeSlot[], ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.slots = [...response.result!].sort((a, b) => {
            return new Date(b.startDate).getTime() - new Date(a.startDate).getTime();
          });
          this.filteredItems = this.slots;
          this.systemMessage = code;
          return of(true);
        } else {
          this.handleErrorResponse(response);
          return of(false);
        }
      })
    ).subscribe({
      next: (result) => {
        console.log(result);
        //if(result) {
        //  thos.setSuccessfulTask();
        //} else {
        //  this.setUn
        //}
      },
      error: (err) => {
        this.logginService.error(err);

      }
    })
  }


  private handleErrorResponse(response: OperationResult<AvailabilityTimeSlot[], ApiDataErrorResponse>): void {

    let code = getStringEnumKeyByValue(MessageCodeType, MessageCodeType.UNKNOWN_ERROR);
    if (isGenericErrorResponse(response.error)) {
      code = this.translationCodes.TC_GENERIC_ERROR_CONFLICT;
    } else if (isValidationErrorResponse(response.error)) {
      code = this.translationCodes.TC_VALIDATION_ERROR;
    } else if (isServerErrorResponse(response.error)) {
      code = getStringEnumKeyByValue(MessageCodeType, response.code);
    } else if (isEmptyErrorResponse(response.error)) {
      code = getStringEnumKeyByValue(MessageCodeType, response.code);
    }

    this.systemMessage = code;
  }

  private handleErrorBoolResponse(response: OperationResult<boolean, ApiDataErrorResponse>): void {

    let code = getStringEnumKeyByValue(MessageCodeType, MessageCodeType.UNKNOWN_ERROR);
    if (isGenericErrorResponse(response.error)) {
      code = this.translationCodes.TC_GENERIC_ERROR_CONFLICT;
    } else if (isValidationErrorResponse(response.error)) {
      code = this.translationCodes.TC_VALIDATION_ERROR;
    } else if (isServerErrorResponse(response.error)) {
      code = getStringEnumKeyByValue(MessageCodeType, response.code);
    } else if (isEmptyErrorResponse(response.error)) {
      code = getStringEnumKeyByValue(MessageCodeType, response.code);
    }

    this.systemMessage = code;
  }


  //
  redirectToRegisterSlot() {
    this.router.navigate([WebRoutes.availability_time_slot_management_register_slot])
  }

  redirectToEditSlot(slot: AvailabilityTimeSlot) {
    this.router.navigate([WebRoutes.availability_time_slot_management_edit_slot], { state: { slot } });
  }
  //
  translate(key: string): string {
    return this.i18nService.translate(key);
  }



  enableAvailabilityTimeSlot(uuid: string) {
    this.schedulerService.enableAvailabilityTimeSlot(uuid).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          const index = this.slots.findIndex(slot => slot.uuid === uuid);
          if (index !== -1) {
            this.slots[index].status = AvailabilityTimeSlotStatusType.ENABLED;
          }
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(true);
        } else {
          this.handleErrorBoolResponse(response);
          return of(false);
        }
      })
    ).subscribe({
      next: (result) => {
        console.log(result);
        //if(result) {
        //  thos.setSuccessfulTask();
        //} else {
        //  this.setUn
        //}
      },
      error: (err) => {
        this.logginService.error(err);

      }
    })
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
          this.handleErrorBoolResponse(response);
          return of(false);
        }
      })
    ).subscribe({
      next: (result) => {
        console.log(result);
        //if(result) {
        //  thos.setSuccessfulTask();
        //} else {
        //  this.setUn
        //}
      },
      error: (err) => {
        this.logginService.error(err);

      }
    })
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
          this.handleErrorBoolResponse(response);
          return of(false);
        }
      })
    ).subscribe({
      next: (result) => {
        console.log(result);



        //if(result) {
        //  thos.setSuccessfulTask();
        //} else {
        //  this.setUn
        //}
      },
      error: (err) => {
        this.logginService.error(err);

      }
    })
  }




}
