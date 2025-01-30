import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../../../cross-cutting/security/authentication/authentication.service';
import { NavigationEnd, Router } from '@angular/router';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { CommonModule } from '@angular/common';
import { AccountData } from '../../../view-model/business-entities/account';
import { Observable } from 'rxjs';
import { ClientHomeComponent } from './roles/client-home/client-home.component';
import { RoleType } from '../../../view-model/business-entities/types/role.types';
import { AssistantHomeComponent } from './roles/assistant-home/assistant-home.component';
import { AdministratorHomeComponent } from './roles/administrator-home/administrator-home.component';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-home',
  imports: [CommonModule, ClientHomeComponent, AssistantHomeComponent, AdministratorHomeComponent],
  standalone: true,
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {

  isAuthenticated: Observable<boolean>
  accountData: Observable<AccountData | null>;
  translationCodes = TranslationCodes;
  roleTypes = RoleType;

  constructor(private authService: AuthenticationService, private router: Router, private i18nService: I18nService, private titleService: Title) {
    this.accountData = this.authService.getAccountData();
    this.isAuthenticated = this.authService.isAuthenticated();
    this.isAuthenticated.subscribe(isAuth => {
      if (!isAuth) {
        this.router.navigate([WebRoutes.login]);
      }
    });
  }

  private updateTitle(): void {
    const titleKey = this.translationCodes.TC_HOME_DIRECTORY;
    const title = this.i18nService.translate(titleKey);
    this.titleService.setTitle(title);
  }

  ngOnInit(): void {

    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.updateTitle();
      }
    });

    this.updateTitle();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate([WebRoutes.login]);
  }


  translate(key: string): string {
    return this.i18nService.translate(key);
  }



}
