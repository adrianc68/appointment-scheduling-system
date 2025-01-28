import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { I18nService } from '../cross-cutting/helper/i18n/i18n.service';
import { AuthenticationService } from '../cross-cutting/security/authentication/authentication.service';
import { CommonModule } from '@angular/common';
import { LoggingService } from '../cross-cutting/operation-management/logginService/logging.service';
import { WebRoutes } from '../cross-cutting/operation-management/model/web-routes.constants';
import { TranslationCodes } from '../cross-cutting/helper/i18n/model/translation-codes.types';
import { LanguageTypes } from '../cross-cutting/helper/i18n/model/languages.types';


@Component({
  selector: 'app-root',
  imports: [
    RouterModule,
    CommonModule,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  translationCodes = TranslationCodes;
  isAuthenticated: boolean = false;

  constructor(private i18nService: I18nService, private loggingService: LoggingService, private authService: AuthenticationService, private router: Router) {
    this.i18nService.setLanguage(this.i18nService.getLanguage());
  }

  ngOnInit(): void {

    this.authService.isAuthenticated().subscribe({
      next: (authenticated: boolean) => {
        this.isAuthenticated = authenticated;
      },
      error: (err) => {
        this.loggingService.warn(err);
      }
    });
  }



  logout(): void {
    this.authService.logout();
    this.router.navigate([WebRoutes.login]);
  }

  changeLanguageToEnglish(): void {
    this.i18nService.setLanguage(LanguageTypes.en_US);
  }

  changeLanguageToSpanish(): void {
    this.i18nService.setLanguage(LanguageTypes.es_MX);
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }


}
