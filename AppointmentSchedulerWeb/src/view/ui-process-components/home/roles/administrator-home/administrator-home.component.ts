import { Component, OnInit } from '@angular/core';
import { I18nService } from '../../../../../cross-cutting/helper/i18n/i18n.service';
import { TranslationCodes } from '../../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { WebRoutes } from '../../../../../cross-cutting/operation-management/model/web-routes.constants';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-administrator-home',
  imports: [CommonModule, RouterModule, MatIconModule],
  standalone: true,
  templateUrl: './administrator-home.component.html',
  styleUrl: './administrator-home.component.scss'
})
export class AdministratorHomeComponent {
  translationCodes = TranslationCodes;
  webRoutes = WebRoutes

  constructor(private i18nService: I18nService) { }


  translate(key: string): string {
    return this.i18nService.translate(key);
  }

}
