import { CommonModule } from '@angular/common';
import { Component, Input, Type, ViewChild, ViewContainerRef } from '@angular/core';

@Component({
  selector: 'app-grid-list',
  imports: [CommonModule],
  standalone: true,

  templateUrl: './grid-list.component.html',
  styleUrl: './grid-list.component.scss'
})
export class GridListComponent {
  @Input() title: string = 'Grid Title';
  @Input() items: any[] = [];
  @Input() cardComponent!: Type<any>;


  //@ViewChild('container', { read: ViewContainerRef }) container!: ViewContainerRef;


}
