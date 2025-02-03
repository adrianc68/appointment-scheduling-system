import { CommonModule } from '@angular/common';
import { Component, ElementRef, HostListener, Input, signal } from '@angular/core';

@Component({
  selector: 'app-dropdown',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './dropdown.component.html',
  styleUrl: './dropdown.component.scss'
})
export class DropdownComponent {
  @Input() label: string = 'Menú';
  isOpen = signal(false);

  constructor(private elRef: ElementRef) { }


  toggleDropdown() {
    this.isOpen.set(!this.isOpen());
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event) {
    if (!this.elRef.nativeElement.contains(event.target)) {
      this.isOpen.set(false);
    }
  }

}
