import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { I18nService } from '../cross-cutting/helper/i18n/i18n.service';
import { LanguageTypes } from '../cross-cutting/helper/i18n/model/languages.types';


@Component({
  selector: 'app-root',
  imports: [
    RouterModule
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'AppointmentSchedulerWeb';

  constructor(private i18nService: I18nService) {
    this.i18nService.setLanguage(LanguageTypes.es_MX);
  }

}
