import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { I18nService } from '../../../cross-cutting/helper/i18n/i18n.service';
import { TranslationCodes } from '../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { WebRoutes } from '../../../cross-cutting/operation-management/model/web-routes.constants';
import { Title } from '@angular/platform-browser';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-landing-page',
  imports: [...SHARED_STANDALONE_COMPONENTS, MatIconModule],
  standalone: true,
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.scss'
})
export class LandingPageComponent implements OnInit {
  translationCodes = TranslationCodes;
  constructor(private titleService: Title, private router: Router, private i18nService: I18nService) {

  }

  ngOnInit(): void {
    this.i18nService.getTranslations().subscribe(translations => {
      const title = translations[TranslationCodes.TC_WELCOME];
      this.titleService.setTitle(title);
    });
  }

  login() {
    this.router.navigate([WebRoutes.login])
  }

  signUp() {
    this.router.navigate([WebRoutes.signup])
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

}
