import { Component, EventEmitter, Input, Output } from '@angular/core';
import { LoadingState } from '../../../model/loading-state.type';
import { CommonModule } from '@angular/common';
import { I18nService } from '../../../../cross-cutting/helper/i18n/i18n.service';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-button-task',
  imports: [CommonModule, MatIconModule],
  standalone: true,
  templateUrl: './button-task.component.html',
  styleUrl: './button-task.component.scss'
})
export class ButtonTaskComponent {
  @Input() loadingState: LoadingState = LoadingState.NO_ACTION_PERFORMED;
  @Input() text: string = "button";
  @Input() type: string = 'button';
  @Input() name: string = '';
  @Input() id: string = '';
  @Input() placeholder: string = '';
  @Input() disabled: boolean = false;
  @Input() class: string = "";
  @Output() clicked = new EventEmitter<void>();
  loadingStateType = LoadingState;

  constructor(private i18nService: I18nService) { }


  onClick() {
    this.clicked.emit();
  }

  get isLoading(): boolean {
    return this.loadingState === LoadingState.LOADING;
  }

  translate(key: string): string {
    return this.i18nService.translate(key);
  }

  setState(state: LoadingState): void {
    this.loadingState = state;
  }

}
