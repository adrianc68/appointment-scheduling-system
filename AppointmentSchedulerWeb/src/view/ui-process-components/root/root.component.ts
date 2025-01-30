import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthenticationService } from '../../../cross-cutting/security/authentication/authentication.service';
import { CommonModule } from '@angular/common';
import { HomeComponent } from '../home/home.component';
import { LandingPageComponent } from '../landing-page/landing-page.component';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { LayoutAuthenticatedComponent } from '../../ui-components/display/layout-authenticated/layout-authenticated.component';

@Component({
  selector: 'app-root-container',
  imports: [CommonModule, RouterModule, LandingPageComponent, LayoutAuthenticatedComponent],
  standalone: true,
  templateUrl: './root.component.html'
})
export class RootComponent {
  isAuthenticated: boolean = false;

  constructor(private authService: AuthenticationService, private loggingService: LoggingService) {
    this.authService.isAuthenticated().subscribe({
      next: (authenticated: boolean) => {
        this.isAuthenticated = authenticated;
      },
      error: (err) => {
        this.loggingService.warn(err);
      }
    });


  }
}
