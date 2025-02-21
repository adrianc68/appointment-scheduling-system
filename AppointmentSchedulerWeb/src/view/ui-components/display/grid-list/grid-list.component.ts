import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, OnInit, SimpleChanges, Type } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { TranslationCodes } from '../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { TranslatePipe } from '../../../../cross-cutting/helper/i18n/translate.pipe';
import { MultiSelectFilterComponent } from '../../input/multi-select-filter/multi-select-filter.component';

@Component({
  selector: 'app-grid-list',
  imports: [CommonModule, FormsModule, MatIconModule, TranslatePipe, MultiSelectFilterComponent],
  standalone: true,

  templateUrl: './grid-list.component.html',
  styleUrl: './grid-list.component.scss'
})
export class GridListComponent implements OnInit, OnChanges {
  TranslationCodes = TranslationCodes;

  @Input() title: string = 'Grid Title';
  @Input() items: any[] = [];
  @Input() cardComponent!: Type<any>;


  //@Input() filters: { key: string, label: string }[] = [];
  //@Input() sortOptions: { key: string, label: string }[] = [];

  searchTerm: string = '';
  filteredItems: any[] = [];
  originalItems: any[] = [];

  // Enable or disable the filters
  setFilters: boolean = false;


  //activeFilters: Set<string> = new Set();
  //activeSort: string = '';
  //sortDirection: 'asc' | 'desc' = 'asc';

  @Input() filters: { key: string, label: string, type: 'boolean' | 'enum' | 'date' | 'datetime' | 'multi', enumValues?: any[] }[] = [];
  activeFilters: { [key: string]: any } = {};

  clearFilters() {
    this.activeFilters = {};
    this.searchTerm = '';
    this.applyFilters();
  }


  toggleFilters() {
    if (this.setFilters) {
      this.clearFilters();
    }
    this.setFilters = !this.setFilters;
  }

  ngOnInit() {
    this.originalItems = [...this.items];
    this.applyFilters();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['items']) {
      this.originalItems = [...this.items];
      this.applyFilters();
    }
  }

  onFilterChange(selectedValues: string[]) {
    console.log('Valores seleccionados:', selectedValues);
  }

  onEnumChange(event: Event, filter: any): void {
    const value = (event.target as HTMLSelectElement).value;
    this.activeFilters[filter.key] = value;
    this.applyFilters();
  }

  applyFilters() {
    let tempItems = [...this.items];

    if (this.searchTerm.trim()) {
      tempItems = tempItems.filter(item =>
        Object.values(item).some(value =>
          value?.toString().toLowerCase().includes(this.searchTerm.toLowerCase())
        )
      );
    }

    if (this.setFilters) {
      Object.keys(this.activeFilters).forEach(filterKey => {
        const filterValue = this.activeFilters[filterKey];
        if (filterValue !== undefined && filterValue !== null && filterValue !== '') {
          tempItems = tempItems.filter(item => {
            const itemValue = item[filterKey];
            const filterType = this.filters.find(f => f.key === filterKey)?.type;
            switch (filterType) {
              case 'boolean':
                return itemValue === filterValue;
              case 'enum':
                return Array.isArray(filterValue) ? filterValue.includes(itemValue) : itemValue === filterValue;
              case 'date':
                return new Date(itemValue).toISOString().split('T')[0] ===
                  new Date(filterValue).toISOString().split('T')[0];
              case 'datetime':
                return new Date(itemValue).toISOString().slice(0, 16) ===
                  new Date(filterValue).toISOString().slice(0, 16);
              default:
                return true;
            }
          });
        }
      });
    }
    this.filteredItems = tempItems;
  }


  toggleFilter(filterKey: string, value: any, type: 'boolean' | 'date' | 'datetime' | 'enum') {
    if (type === 'boolean') {
      this.activeFilters[filterKey] = !this.activeFilters[filterKey];
    } else if (type === 'enum' || type === 'date' || type === 'datetime') {
      this.activeFilters[filterKey] = this.activeFilters[filterKey] === value ? null : value;
    }
    this.applyFilters();
  }

}
