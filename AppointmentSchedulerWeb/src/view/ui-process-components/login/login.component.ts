import { Component } from '@angular/core';
import { HttpClientService } from '../../../cross-cutting/communication/http-client-service/http-client.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { catchError, of, tap } from 'rxjs';
import { HttpClientAdapter } from '../../../cross-cutting/communication/http-client-adapter-service/http-client-adapter.service';
import { JwtTokenDTO } from '../../../view-model/dtos/jwt-token.dto';
import { GenericError } from '../../../cross-cutting/communication/model/generic-error';

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

  constructor(private httpClientService: HttpClientAdapter) { }

  onSubmit() {
    const loginData = {
      account: this.email,
      password: this.password
    };
    this.httpClientService.post<JwtTokenDTO>("http://localhost:5120/api/v1/Auth/login", loginData).subscribe({
      next: (response) => {
        console.log("next");
        console.log(response);
      },
      error: (err) => {
        console.log("errror");
        console.log(err);
      }

    });


  }

}
