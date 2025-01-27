import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { AuthenticationService } from '../../../cross-cutting/security/authentication/authentication.service';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { finalize, of, switchMap, take } from 'rxjs';

@Component({
  selector: 'app-login',
  imports: [FormsModule, CommonModule],
  standalone: true,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {

  account: string = '';
  password: string = '';
  systemMessage?: string = '';
  isLoading: boolean = false;
  dataLoaded: boolean = false;

  constructor(private authenticationService: AuthenticationService, private i18nService: I18nService, private logginService: LoggingService) { }

  onSubmit() {
    if (!this.isLoading) {
      this.isLoading = true;
      this.dataLoaded = false;
      this.systemMessage = '';

      this.authenticationService.loginJwt(this.account, this.password).pipe(
        switchMap((response) => {
          console.log(response);
          if (response.isSuccessful && response.code === MessageCodeType.OK) {
            let code = getStringEnumKeyByValue(MessageCodeType, response.code);
            this.systemMessage = this.i18nService.translate(code!);
            return this.authenticationService.getAccountDataFromServer();
          } else {
            let code = getStringEnumKeyByValue(MessageCodeType, response.code);
            this.systemMessage = this.i18nService.translate(code!);
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
          }
        },
        error: (err) => {
          this.logginService.error(err);
        }
      });
    }
  }


}
