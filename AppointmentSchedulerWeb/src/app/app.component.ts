import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { I18nService } from '../cross-cutting/helper/i18n/i18n.service';
import { LanguageTypes } from '../cross-cutting/helper/i18n/model/languages.types';
import { AuthenticationService } from '../cross-cutting/security/authentication/authentication.service';
import { Observable, of, pipe } from 'rxjs';
import { CommonModule } from '@angular/common';


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
  isAuthenticated: boolean = false;

  constructor(private i18nService: I18nService, private authService: AuthenticationService, private router: Router) {
    this.i18nService.setLanguage(LanguageTypes.es_MX);
  }

  ngOnInit(): void {

    this.authService.isAuthenticated().subscribe({
      next: (authenticated: boolean) => {
        this.isAuthenticated = authenticated;  // Asigna el valor de autenticación
      },
      error: (err) => {
        console.error('Error en la autenticación', err);
      }
    });


  }



  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }


}
