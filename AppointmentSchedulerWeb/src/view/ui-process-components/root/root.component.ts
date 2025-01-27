import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthenticationService } from '../../../cross-cutting/security/authentication/authentication.service';
import { CommonModule } from '@angular/common';
import { HomeComponent } from '../home/home.component';
import { LandingPageComponent } from '../landing-page/landing-page.component';

@Component({
  selector: 'app-root-container',
  imports: [CommonModule, RouterModule, HomeComponent, LandingPageComponent],
  templateUrl: './root.component.html'
})
export class RootComponent {
  isAuthenticated: boolean = false;

  constructor(private authService: AuthenticationService, private router: Router) {
    this.authService.isAuthenticated().subscribe({
      next: (authenticated: boolean) => {
        this.isAuthenticated = authenticated;  // Asigna el valor de autenticación
      },
      error: (err) => {
        console.error('Error en la autenticación', err);
      }
    });


  }
}
