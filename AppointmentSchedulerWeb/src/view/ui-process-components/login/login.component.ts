import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { JwtTokenDTO } from '../../../view-model/dtos/jwt-token.dto';
import { AccountService } from '../../../model/communication-components/account.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule, CommonModule],
  standalone: true,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {

  email: string = '';
  password: string = '';

  constructor(private accountService: AccountService) { }

  onSubmit() {


    this.accountService.loginJwtAuth(this.email, this.password).subscribe({
      next: (response) => {
        console.log(response);
      },
      error: (err) => {
        console.log(err);
      }
    })



  }

}
