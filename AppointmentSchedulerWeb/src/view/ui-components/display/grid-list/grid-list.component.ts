import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, OnInit, SimpleChanges, Type } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { TranslationCodes } from '../../../../cross-cutting/helper/i18n/model/translation-codes.types';
import { TranslatePipe } from '../../../../cross-cutting/helper/i18n/translate.pipe';

@Component({
  selector: 'app-grid-list',
  imports: [CommonModule, FormsModule, MatIconModule, TranslatePipe],
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

  @Input() filters: { key: string, label: string, type: 'boolean' | 'enum' | 'date' | 'datetime', enumValues?: any[] }[] = [];
  activeFilters: { [key: string]: any } = {};



  toggleFilters() {
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
            if (typeof filterValue === 'boolean') {
              return itemValue === filterValue;
            }

            if (typeof itemValue === "string" && typeof filterValue === "string") {
              const itemDateOnly = new Date(itemValue).toISOString().split('T')[0];
              const filterDateOnly = new Date(filterValue).toISOString().split('T')[0];
              return itemDateOnly === filterDateOnly;
            }

            if (Array.isArray(filterValue)) {
              return filterValue.includes(itemValue);
            }


            return true;
          });
        }
      });

    }

    this.filteredItems = tempItems;
  }




}
