import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { WebRoutes } from '../../../../cross-cutting/operation-management/model/web-routes.constants';
import { TranslationCodes } from '../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { I18nService } from '../../../../cross-cutting/helper/i18n/i18n.service';
import { AuthenticationService } from '../../../../cross-cutting/security/authentication/authentication.service';
import { Observable } from 'rxjs';
import { AccountData } from '../../../../view-model/business-entities/account';
import { RoleType } from '../../../../view-model/business-entities/types/role.types';

@Component({
  selector: 'app-side-bar',
  imports: [CommonModule, RouterModule],
  standalone: true,
  templateUrl: './side-bar.component.html',
  styleUrl: './side-bar.component.scss'
})
export class SideBarComponent {
  webRoutes = WebRoutes
  translationCodes = TranslationCodes
  accountData: Observable<AccountData | null>;
  roleTypes = RoleType;

  constructor(private i18nService: I18nService, private authService: AuthenticationService) {
    this.accountData = this.authService.getAccountData();
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }





}
