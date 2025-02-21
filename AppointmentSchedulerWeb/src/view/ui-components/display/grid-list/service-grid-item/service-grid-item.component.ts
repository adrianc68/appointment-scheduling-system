import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { TranslatePipe } from '../../../../../cross-cutting/helper/i18n/translate.pipe';
import { Service } from '../../../../../view-model/business-entities/service';
import { TranslationCodes } from '../../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { Router } from '@angular/router';
import { WebRoutes } from '../../../../../cross-cutting/operation-management/model/web-routes.constants';

@Component({
  selector: 'app-service-grid-item',
  imports: [CommonModule, MatIconModule, TranslatePipe],
  standalone: true,
  templateUrl: './service-grid-item.component.html',
  styleUrl: './service-grid-item.component.scss'
})
export class ServiceGridItemComponent {
  @Input() data!: Service;
  TranslationCodes = TranslationCodes;

  constructor(private router: Router) {

  }

  redirectToEditService(service: Service) {
    this.router.navigate([WebRoutes.service_management_edit_service], { state: { service } });
  }

}
