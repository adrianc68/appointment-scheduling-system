import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslationCodes } from '../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { RoleType } from '../../../../view-model/business-entities/types/role.types';
import { NavBarComponent } from '../../navigation/nav-bar/nav-bar.component';
import { SideBarComponent } from '../../navigation/side-bar/side-bar.component';

@Component({
  selector: 'app-layout-authenticated',
  imports: [CommonModule, RouterModule, NavBarComponent, SideBarComponent],
  standalone: true,
  templateUrl: './layout-authenticated.component.html',
  styleUrl: './layout-authenticated.component.scss'
})
export class LayoutAuthenticatedComponent {
  translationCodes = TranslationCodes;
  roleTypes = RoleType;
  sidebarOpen: boolean = false;

  constructor() { }

  toggleSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
  }

  onSidebarStateChange(isOpen: boolean) {
    this.sidebarOpen = isOpen;
  }

}
