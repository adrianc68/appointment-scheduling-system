import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { Client } from '../../../../../view-model/business-entities/client';
import { Router } from '@angular/router';
import { WebRoutes } from '../../../../../cross-cutting/operation-management/model/web-routes.constants';
import { TranslatePipe } from '../../../../../cross-cutting/helper/i18n/translate.pipe';
import { TranslationCodes } from '../../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-client-card',
  imports: [CommonModule, MatIconModule, TranslatePipe],
  standalone: true,
  templateUrl: './client-card.component.html',
  styleUrl: './client-card.component.scss'
})
export class ClientCardComponent {
  @Input() data!: Client;
  TranslationCodes = TranslationCodes;

  constructor(private router: Router) {

  }

  redirectToEditClient(client: Client) {
    this.router.navigate([WebRoutes.client_management_edit_client], { state: { client } });
  }

}
