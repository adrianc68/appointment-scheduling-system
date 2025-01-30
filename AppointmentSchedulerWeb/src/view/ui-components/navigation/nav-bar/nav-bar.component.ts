import { Component, EventEmitter, Output } from '@angular/core';
import { SHARED_STANDALONE_COMPONENTS } from '../../shared-components';
import { CommonModule } from '@angular/common';
import { AuthenticationService } from '../../../../cross-cutting/security/authentication/authentication.service';
import { Observable } from 'rxjs';
import { AccountData } from '../../../../view-model/business-entities/account';
import { WebRoutes } from '../../../../cross-cutting/operation-management/model/web-routes.constants';
import { Router } from '@angular/router';
import { I18nService } from '../../../../cross-cutting/helper/i18n/i18n.service';
import { TranslationCodes } from '../../../../cross-cutting/helper/i18n/model/translation-codes.types';

@Component({
  selector: 'app-nav-bar',
  imports: [CommonModule, ...SHARED_STANDALONE_COMPONENTS],
  standalone: true,
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.scss'
})
export class NavBarComponent {
  isAuthenticated: Observable<boolean>
  accountData: Observable<AccountData | null>;
  translationCodes = TranslationCodes;

  @Output() menuToggle = new EventEmitter<void>();

  constructor(private authService: AuthenticationService, private router: Router, private i18nService: I18nService) {
    this.accountData = this.authService.getAccountData();
    this.isAuthenticated = this.authService.isAuthenticated();
    this.isAuthenticated.subscribe(isAuth => {
      if (!isAuth) {
        this.router.navigate([WebRoutes.login]);
      }
    });
  }

  toggleSidebar() {
    this.menuToggle.emit();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate([WebRoutes.login]);
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }


}
