import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { I18nService } from '../../../../cross-cutting/helper/i18n/i18n.service';

@Component({
  selector: 'app-error-validation-message-form',
  imports: [FormsModule, CommonModule],
  standalone: true,
  templateUrl: './error-validation-message-form.component.html',
  styleUrl: './error-validation-message-form.component.scss'
})
export class ErrorValidationMessageFormComponent {

  @Input() messages: string[] = [];

  constructor(private i18nService: I18nService) {}

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

}
