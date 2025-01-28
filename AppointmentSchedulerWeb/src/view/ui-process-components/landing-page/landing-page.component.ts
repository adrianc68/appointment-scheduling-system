import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';

@Component({
  selector: 'app-landing-page',
  imports: [...SHARED_STANDALONE_COMPONENTS],
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.scss'
})
export class LandingPageComponent {
  translationCodes = TranslationCodes;
  constructor(private router: Router, private i18nService: I18nService) {

  }

  login() {
    this.router.navigate(["/login"])
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

}
