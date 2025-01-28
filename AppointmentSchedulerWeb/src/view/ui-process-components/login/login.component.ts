import { Component, NgZone } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { AuthenticationService } from '../../../cross-cutting/security/authentication/authentication.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { finalize, of, switchMap, take } from 'rxjs';
import { Router } from '@angular/router';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';

@Component({
  selector: 'app-login',
  imports: [FormsModule, CommonModule, ...SHARED_STANDALONE_COMPONENTS],
  standalone: true,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  translationCodes = TranslationCodes;
  account: string = '';
  password: string = '';
  systemMessage?: string = '';
  isLoading: boolean = false;
  dataLoaded: boolean = false;

  constructor(private authenticationService: AuthenticationService, private router: Router, private ngZone: NgZone, private i18nService: I18nService, private logginService: LoggingService) { }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  onSubmit() {
    if (!this.isLoading) {
      this.isLoading = true;
      this.dataLoaded = false;
      this.systemMessage = "";

      this.authenticationService.loginJwt(this.account, this.password).pipe(
        switchMap((response) => {
          if (response.isSuccessful && response.code === MessageCodeType.OK) {
            let code = getStringEnumKeyByValue(MessageCodeType, response.code);
            this.systemMessage = code;
            return this.authenticationService.getAccountDataFromServer();
          } else {
            let code = getStringEnumKeyByValue(MessageCodeType, response.code);
            this.systemMessage = code;
            return of(false);
          }
        }),
        switchMap((isDataReceived) => {
          if (isDataReceived) {
            return this.authenticationService.getAccountData().pipe(
              take(1),
            );
          }
          return of(null);
        })
      ).pipe(finalize(() => {
        this.isLoading = false;
      })).subscribe({
        next: (accountData) => {
          if (accountData) {
            this.dataLoaded = true;
            this.ngZone.run(() => {
              this.router.navigate([WebRoutes.root]);
            });
          }
        },
        error: (err) => {
          this.logginService.error(err);
        }
      });
    }
  }


}
