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
  @Input() filters: { key: string, label: string }[] = [];
  @Input() sortOptions: { key: string, label: string }[] = [];

  searchTerm: string = '';

  filteredItems: any[] = [];
  originalItems: any[] = [];

  activeFilters: Set<string> = new Set();
  activeSort: string = '';
  sortDirection: 'asc' | 'desc' = 'asc';


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

  toggleFilter(filterKey: string) {
    if (this.activeFilters.has(filterKey)) {
      this.activeFilters.delete(filterKey);
    } else {
      this.activeFilters.add(filterKey);
    }
    this.applyFilters();
  }

  toggleSort(sortKey: string) {
    if (this.activeSort === sortKey) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.activeSort = sortKey;
      this.sortDirection = 'asc';
    }
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

    if (this.activeFilters.size > 0) {
      tempItems = tempItems.filter(item =>
        Array.from(this.activeFilters).every(filterKey => item[filterKey])
      );
    }

    if (this.activeSort) {
      tempItems.sort((a, b) => {
        const valA = a[this.activeSort]?.toString().toLowerCase();
        const valB = b[this.activeSort]?.toString().toLowerCase();
        return this.sortDirection === 'asc' ? valA.localeCompare(valB) : valB.localeCompare(valA);
      });
    }

    this.filteredItems = tempItems;
  }

}
