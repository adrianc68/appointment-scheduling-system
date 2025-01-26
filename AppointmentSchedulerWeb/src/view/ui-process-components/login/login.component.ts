import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AccountService } from '../../../model/communication-components/account.service';
import { MessageCodeType } from '../../../cross-cutting/communication/model/message-code.types';
import { LocalStorageService } from '../../../cross-cutting/security/local-storage/local-storage.service';
import { getStringEnumKeyByValue } from '../../../cross-cutting/helper/enum-utils/enum.utils';

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
  systemMessage: string = '';

  constructor(private accountService: AccountService, private localStorageService: LocalStorageService) { }

  onSubmit() {
    this.systemMessage = '';

    this.accountService.loginJwtAuth(this.account, this.password).subscribe({
      next: (response) => {
        if (response.isSuccessful) {
          if (response.code === MessageCodeType.OK) {
            this.localStorageService.setItem("jwtToken", response.result!.token);
            this.systemMessage = getStringEnumKeyByValue(MessageCodeType, response.code) || "NON";
          }
             //tokenData = response.result;
        } else {
            this.systemMessage = getStringEnumKeyByValue(MessageCodeType, response.code) || "NON";
        }
      },
      error: (err) => {
        console.log(err);
      }
    })



  }

}
