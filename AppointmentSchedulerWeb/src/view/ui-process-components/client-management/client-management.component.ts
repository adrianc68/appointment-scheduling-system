import { Component } from '@angular/core';
import { ClientService } from '../../../model/communication-components/client.service';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { ApiDataErrorResponse } from '../../../cross-cutting/communication/model/api-response.error';
import { Client } from '../../../view-model/business-entities/client';
import { catchError, map, of } from 'rxjs';
import { OperationResult } from '../../../cross-cutting/communication/model/operation-result.response';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { CommonModule } from '@angular/common';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { Router } from '@angular/router';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';
import { ClientCardComponent } from '../../ui-components/display/card/client-card/client-card.component';
import { TranslatePipe } from '../../../cross-cutting/helper/i18n/translate.pipe';
import { AccountStatusType } from '../../../view-model/business-entities/types/account-status.types';
import { MatIconModule } from '@angular/material/icon';
import { ErrorUIService } from '../../../cross-cutting/communication/handle-error-service/error-ui.service';

@Component({
  selector: 'app-client-management',
  imports: [CommonModule, ...SHARED_STANDALONE_COMPONENTS, TranslatePipe, MatIconModule],
  standalone: true,
  templateUrl: './client-management.component.html',
  styleUrl: './client-management.component.scss'
})
export class ClientManagementComponent {
  translationCodes = TranslationCodes;
  clients: Client[] = [];
  clientCard = ClientCardComponent;
  AccountStatusType = AccountStatusType;
  accountStatusType = AccountStatusType;

  constructor(private clientService: ClientService, private i18nService: I18nService, private logginService: LoggingService, private router: Router, private errorUIService: ErrorUIService) {
    this.getClientList();
  }

  private getClientList(): void {
    this.clientService.getClientList().pipe(
      map((response: OperationResult<Client[], ApiDataErrorResponse>) => {
        if (response.isSuccessful && response.code === MessageCodeType.OK && response.result) {
          return response.result;
        } else {
          this.errorUIService.handleError(response);
          return [];
        }
      }),
      catchError(err => {
        this.logginService.error(err);
        this.errorUIService.handleError(err);
        return of([]);
      })
    ).subscribe((clients: Client[]) => {
      this.clients = clients;
    });
  }


  get enabledClientsCount(): number {
    return this.clients.filter(client => client.status === AccountStatusType.ENABLED).length;
  }

  get disabledClientsCount(): number {
    return this.clients.filter(client => client.status === AccountStatusType.DISABLED).length;
  }


  redirectToRegisterClient() {
    this.router.navigate([WebRoutes.client_management_register_client])
  }

  redirectToEditClient(client: Client) {
    this.router.navigate([WebRoutes.client_management_edit_client], { state: { client } });
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

}
