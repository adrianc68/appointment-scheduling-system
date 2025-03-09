import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterModule } from '@angular/router';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';

@Component({
  selector: 'app-landing-page-header',
  imports: [CommonModule, MatIconModule, RouterModule],
  standalone: true,
  templateUrl: './landing-page-header.component.html',
  styleUrl: './landing-page-header.component.scss'
})
export class LandingPageHeaderComponent {
  menuOpen = false;
  webRoutes = WebRoutes;

  constructor(private router: Router) { }

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }

}
