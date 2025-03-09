import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { TranslatePipe } from '../../../../../cross-cutting/helper/i18n/translate.pipe';
import { TranslationCodes } from '../../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { ServiceAssignment } from '../../../../../view-model/business-entities/service-assignment';

@Component({
  selector: 'app-service-assigned-grid-item',
  imports: [CommonModule, MatIconModule, TranslatePipe],
  standalone: true,
  templateUrl: './service-assigned-grid-item.component.html',
  styleUrl: './service-assigned-grid-item.component.scss'
})
export class ServiceAssignedGridItemComponent {
  TranslationCodes = TranslationCodes;
  @Input() data!: ServiceAssignment;

}
