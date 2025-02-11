import { CommonModule } from '@angular/common';
import { Component, HostListener, Input, ElementRef, EventEmitter, Output } from '@angular/core';
import { RouterModule } from '@angular/router';
import { WebRoutes } from '../../../../cross-cutting/operation-management/model/web-routes.constants';
import { TranslationCodes } from '../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { I18nService } from '../../../../cross-cutting/helper/i18n/i18n.service';
import { AuthenticationService } from '../../../../cross-cutting/security/authentication/authentication.service';
import { Observable } from 'rxjs';
import { AccountData } from '../../../../view-model/business-entities/account';
import { RoleType } from '../../../../view-model/business-entities/types/role.types';
import { MatIcon, MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-side-bar',
  imports: [CommonModule, RouterModule, MatIconModule],
  standalone: true,
  templateUrl: './side-bar.component.html',
  styleUrl: './side-bar.component.scss'
})
export class SideBarComponent {
  @Input() isOpen = false;

  @Output() isOpenChange = new EventEmitter<boolean>();


  webRoutes = WebRoutes
  translationCodes = TranslationCodes
  accountData: Observable<AccountData | null>;
  roleTypes = RoleType;


  constructor(private i18nService: I18nService, private authService: AuthenticationService, private elementRef: ElementRef) {
    this.accountData = this.authService.getAccountData();
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  logout(): void {
    this.authService.logout();
  }


  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event) {
    if (this.isOpen && this.isMobile() && !this.elementRef.nativeElement.contains(event.target)) {
      this.isOpen = false;
      this.isOpenChange.emit(this.isOpen);

    }
  }

  private isMobile(): boolean {
    return window.innerWidth <= 768;
  }


}
