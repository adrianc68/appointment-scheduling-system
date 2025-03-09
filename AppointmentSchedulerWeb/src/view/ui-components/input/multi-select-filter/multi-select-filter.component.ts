import { CommonModule } from '@angular/common';
import { Component, ElementRef, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { TranslationCodes } from '../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { TranslatePipe } from '../../../../cross-cutting/helper/i18n/translate.pipe';
import { OverlayModule } from '@angular/cdk/overlay';

@Component({
  selector: 'app-multi-select-filter',
  imports: [CommonModule, FormsModule, MatIconModule, TranslatePipe, OverlayModule],
  standalone: true,
  templateUrl: './multi-select-filter.component.html',
  styleUrl: './multi-select-filter.component.scss'
})
export class MultiSelectFilterComponent {
  @Input() label: string = 'Filtro';
  @Input() options: string[] = [];
  @Output() selectionChanged = new EventEmitter<string[]>();

  TranslationCodes = TranslationCodes;
  selectedOptions: string[] = [];
  searchTerm: string = '';
  dropdownOpen: boolean = false;

  constructor(private eRef: ElementRef) { }

  @HostListener('document:click', ['$event'])
  closeDropdown(event: Event) {
    if (this.dropdownOpen && !this.eRef.nativeElement.contains(event.target)) {
      this.dropdownOpen = false;
    }
  }


  toggleDropdown() {
    this.dropdownOpen = !this.dropdownOpen;
  }


  //toggleDropdown() {
  //  this.dropdownOpen = !this.dropdownOpen;
  //  if (this.dropdownOpen) {
  //    setTimeout(() => {
  //      const dropdown = document.querySelector('.dropdown-menu') as HTMLElement;
  //      const button = document.querySelector('.input') as HTMLElement;
  //      if (dropdown && button) {
  //        const rect = button.getBoundingClientRect();
  //        dropdown.style.top = `${rect.bottom + window.scrollY}px`;
  //        dropdown.style.left = `${rect.left}px`;
  //      }
  //    }, 0);
  //  }
  //}

  toggleSelection(option: string) {
    if (this.selectedOptions.includes(option)) {
      this.selectedOptions = this.selectedOptions.filter(item => item !== option);
    } else {
      this.selectedOptions.push(option);
    }
    this.selectionChanged.emit(this.selectedOptions);
  }

  toggleAll() {
    if (this.areAllSelected()) {
      this.selectedOptions = [];
    } else {
      this.selectedOptions = [...this.options];
    }
    this.selectionChanged.emit(this.selectedOptions);
  }

  areAllSelected(): boolean {
    return this.selectedOptions.length === this.options.length;
  }

  getSelectedText(): string {
    return this.selectedOptions.length ? TranslationCodes.TC_SELECTED_DATA_LABEL : TranslationCodes.TC_SELECT_OPTION_PLACEHOLDER_LABEL;
  }

  filteredOptions(): string[] {
    return this.options.filter(option =>
      option.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }
}
