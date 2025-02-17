import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { ThemeService } from './cross-cutting/operation-management/configService/theme.service';


const themeService = new ThemeService();
themeService.setTheme(themeService.getCurrentTheme());

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
