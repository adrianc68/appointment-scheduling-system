import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { SHARED_STANDALONE_COMPONENTS } from '../../ui-components/shared-components';
import { ThemeService } from '../../../cross-cutting/operation-management/configService/theme.service';

@Component({
  selector: 'app-config-management',
  imports: [CommonModule, ...SHARED_STANDALONE_COMPONENTS],
  standalone: true,
  templateUrl: './config-management.component.html',
  styleUrl: './config-management.component.scss'
})
export class ConfigManagementComponent {
  constructor(private themeService: ThemeService) { }

  toggleTheme() {
    this.themeService.toggleTheme();
  }

  get currentTheme(): 'light' | 'dark' {
    return this.themeService.getCurrentTheme();
  }
}
