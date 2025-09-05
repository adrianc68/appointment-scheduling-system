import { CommonModule } from '@angular/common';
import { Location } from '@angular/common'; // 👈 Import correcto
import { Component } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { TranslationCodes } from '../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { RoleType } from '../../../../view-model/business-entities/types/role.types';
import { NavBarComponent } from '../../navigation/nav-bar/nav-bar.component';
import { SideBarComponent } from '../../navigation/side-bar/side-bar.component';
import { BreadcrumbsComponent } from '../../navigation/breadcrumbs/breadcrumbs.component';

@Component({
  selector: 'app-layout-authenticated',
  imports: [CommonModule, RouterModule, NavBarComponent, SideBarComponent, BreadcrumbsComponent],
  standalone: true,
  templateUrl: './layout-authenticated.component.html',
  styleUrl: './layout-authenticated.component.scss'
})
export class LayoutAuthenticatedComponent {
  translationCodes = TranslationCodes;
  roleTypes = RoleType;
  sidebarOpen: boolean = false;


  toggleSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
  }

  onSidebarStateChange(isOpen: boolean) {
    this.sidebarOpen = isOpen;
  }

  showBackButton = false;

  constructor(private location: Location, private router: Router) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        // Mostrar el botón si NO estamos en la página principal
        this.showBackButton = event.urlAfterRedirects !== '/';
      }
    });
  }

  goBack() {
    this.location.back();
  }

}
