import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { TaskStateManagerService } from '../../model/task-state-manager.service';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { LoadingState } from '../../model/loading-state.type';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { AccountService } from '../../../model/communication-components/account.service';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { catchError, map, Observable, of, switchMap } from 'rxjs';
import { ApiDataErrorResponse} from '../../../cross-cutting/communication/model/api-response.error';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { Client } from '../../../view-model/business-entities/client';
import { ClientService } from '../../../model/communication-components/client.service';
import { MatIconModule } from '@angular/material/icon';
import { ErrorUIService } from '../../../cross-cutting/communication/handle-error-service/error-ui.service';

@Component({
  selector: 'app-edit-client',
  imports: [FormsModule, CommonModule, ...SHARED_STANDALONE_COMPONENTS, MatIconModule],
  providers: [TaskStateManagerService],
  standalone: true,
  templateUrl: './edit-client.component.html',
  styleUrl: './edit-client.component.scss'
})
export class EditClientComponent {
  translationCodes = TranslationCodes;
  errorValidationMessage: { [field: string]: string[] } = {};
  systemMessage?: string = '';
  loadingState: LoadingState = LoadingState.NO_ACTION_PERFORMED;

  client: Client;

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  constructor(private titleService: Title, private router: Router, private i18nService: I18nService, private loggingService: LoggingService, private accountService: AccountService, private clientService: ClientService, private errorUIService: ErrorUIService) {
    this.client = this.router.getCurrentNavigation()?.extras.state?.["client"];
  }

  onSubmit() {

    if (this.loadingState == LoadingState.LOADING || this.loadingState == LoadingState.WORK_DONE) {
      return;
    }

    if (this.loadingState === LoadingState.SUCCESSFUL_TASK) {
      this.loadingState = LoadingState.WORK_DONE;
      return;
    }

    this.systemMessage = "";
    this.errorValidationMessage = {};
    this.loadingState = LoadingState.LOADING;

    this.editClient(this.client).subscribe(result => {

      if (result) {
        this.loadingState = LoadingState.SUCCESSFUL_TASK;
      } else {
        this.loadingState = LoadingState.UNSUCCESSFUL_TASK;
      }

    })
  }

  editClient(client: Client): Observable<boolean> {
    return this.clientService.editClient(client).pipe(
      map((response: OperationResult<boolean, ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          return true;
        } else {
          this.errorUIService.handleError(response);
          const validationErrors = this.errorUIService.getValidationErrors(response);
          Object.entries(validationErrors).forEach(([field, messages]) => {
            this.setErrorValidationMessage(field, messages);
          });
          return false;
        }
      }),
      catchError(err => {
        this.loggingService.error(err);
        return of(false);
      })
    );
  }

  disableClient(uuid: string) {
    this.clientService.disableClient(uuid).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(true);
        } else {
          this.errorUIService.handleError(response);
          return of(false);
        }
      }),
    ).subscribe({
      next: (result) => {
        if (result) {
          //this.loadingState = LoadingState.SUCCESSFUL_TASK;
        } else {
          //this.loadingState = LoadingState.UNSUCCESSFUL_TASK;
        }
      },
      error: (err) => {
        this.loggingService.error(err);
        //this.loadingState = LoadingState.UNSUCCESSFUL_TASK;
      }
    });
  }

  enableClient(uuid: string) {
    this.clientService.enableClient(uuid).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(true);
        } else {
          this.errorUIService.handleError(response);
          return of(false);
        }
      }),
    ).subscribe({
      next: (result) => {
        if (result) {
          //this.loadingState = LoadingState.SUCCESSFUL_TASK;
        } else {
          //this.loadingState = LoadingState.UNSUCCESSFUL_TASK;
        }
      },
      error: (err) => {
        this.loggingService.error(err);
        //this.loadingState = LoadingState.UNSUCCESSFUL_TASK;
      }
    });
  }

  deleteClient(uuid: string) {
    this.clientService.deleteClient(uuid).pipe(
      switchMap((response: OperationResult<boolean, ApiDataErrorResponse>): Observable<boolean> => {
        if (response.isSuccessful && response.code === MessageCodeType.OK) {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = code;
          return of(true);
        } else {
          this.errorUIService.handleError(response);
          return of(false);
        }
      }),
    ).subscribe({
      next: (result) => {
        if (result) {
          //this.loadingState = LoadingState.SUCCESSFUL_TASK;
        } else {
          //this.loadingState = LoadingState.UNSUCCESSFUL_TASK;
        }
      },
      error: (err) => {
        this.loggingService.error(err);
          //this.loadingState = LoadingState.UNSUCCESSFUL_TASK;
      }
    });
  }


  private setErrorValidationMessage(key: string, value: string[]) {
    this.errorValidationMessage[key.toLowerCase()] = value;
  }


}
