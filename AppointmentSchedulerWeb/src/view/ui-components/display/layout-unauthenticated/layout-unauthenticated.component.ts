import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { TranslationCodes } from '../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { RouterModule } from '@angular/router';
import { LandingPageHeaderComponent } from '../../../ui-process-components/landing-page-header/landing-page-header.component';

@Component({
  selector: 'app-layout-unauthenticated',
  imports: [CommonModule, RouterModule, LandingPageHeaderComponent],
  standalone: true,
  templateUrl: './layout-unauthenticated.component.html',
  styleUrl: './layout-unauthenticated.component.scss'
})
export class LayoutUnauthenticatedComponent {
  translationCodes = TranslationCodes;
}
