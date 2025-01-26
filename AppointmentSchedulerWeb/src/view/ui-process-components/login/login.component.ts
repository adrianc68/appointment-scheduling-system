import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AccountService } from '../../../model/communication-components/account.service';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { LocalStorageService } from '../../../cross-cutting/security/local-storage/local-storage.service';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { AuthenticationService } from '../../../cross-cutting/security/authentication/authentication.service';

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

  constructor(private authenticationService: AuthenticationService, private i18nService: I18nService) { }

  onSubmit() {
    this.systemMessage = '';

    this.authenticationService.loginJwt(this.account, this.password).subscribe({
      next: (response) => {
        console.log("onsubmit");
        console.log(response);
        if (response.isSuccessful) {
          if (response.code === MessageCodeType.OK) {
            let code = getStringEnumKeyByValue(MessageCodeType, response.code);
            this.systemMessage = this.i18nService.translate(code!);
          }
        } else {
          let code = getStringEnumKeyByValue(MessageCodeType, response.code);
          this.systemMessage = this.i18nService.translate(code!);
        }
      },
      error: (err) => {
        console.log(err);
      }
    })



  }

}
