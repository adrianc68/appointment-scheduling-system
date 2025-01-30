import { Component, OnInit } from '@angular/core';
import { I18nService } from '../../../../../cross-cutting/helper/i18n/i18n.service';
import { TranslationCodes } from '../../../../../cross-cutting/helper/i18n/model/translation-codes.types';

@Component({
  selector: 'app-administrator-home',
  imports: [],
  standalone: true,
  templateUrl: './administrator-home.component.html',
  styleUrl: './administrator-home.component.scss'
})
export class AdministratorHomeComponent {
  translationCodes = TranslationCodes;


  constructor(private i18nService: I18nService) { }


  translate(key: string): string {
    return this.i18nService.translate(key);
  }

}
