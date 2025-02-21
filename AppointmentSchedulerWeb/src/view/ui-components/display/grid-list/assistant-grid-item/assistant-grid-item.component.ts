import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { TranslatePipe } from '../../../../../cross-cutting/helper/i18n/translate.pipe';
import { MatIconModule } from '@angular/material/icon';
import { Assistant } from '../../../../../view-model/business-entities/assistant';
import { TranslationCodes } from '../../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { Router } from '@angular/router';
import { WebRoutes } from '../../../../../cross-cutting/operation-management/model/web-routes.constants';

@Component({
  selector: 'app-assistant-grid-item',
  imports: [CommonModule, TranslatePipe, MatIconModule],
  standalone: true,
  templateUrl: './assistant-grid-item.component.html',
  styleUrl: './assistant-grid-item.component.scss'
})
export class AssistantGridItemComponent {
  @Input() data!: Assistant;
  TranslationCodes = TranslationCodes;

  constructor(private router: Router) {

  }

  redirectToEditClient(assistant: Assistant) {
    this.router.navigate([WebRoutes.assistant_management_edit_assistant], { state: { assistant } });
  }
}
