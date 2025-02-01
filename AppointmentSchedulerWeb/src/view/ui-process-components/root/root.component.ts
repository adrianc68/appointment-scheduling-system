import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthenticationService } from '../../../cross-cutting/security/authentication/authentication.service';
import { CommonModule } from '@angular/common';
import { LandingPageComponent } from '../landing-page/landing-page.component';
import { LoggingService } from '../../../cross-cutting/operation-management/logginService/logging.service';
import { LayoutAuthenticatedComponent } from '../../ui-components/display/layout-authenticated/layout-authenticated.component';

@Component({
  selector: 'app-root-container',
  imports: [CommonModule, RouterModule, LandingPageComponent, LayoutAuthenticatedComponent],
  standalone: true,
  templateUrl: './root.component.html'
})
export class RootComponent implements OnInit {
  isAuthenticated: boolean = false;
  messages: string[] = [];

  constructor(private authService: AuthenticationService, private loggingService: LoggingService) { }

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


}
