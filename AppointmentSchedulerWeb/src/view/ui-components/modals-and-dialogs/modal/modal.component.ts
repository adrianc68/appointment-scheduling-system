import {
  Component,
  Inject,
  OnInit,
  ViewChild,
  ViewContainerRef,
  Type,
} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { NotificationComponent } from '../../notification/notification/notification/notification.component';

@Component({
  selector: 'app-modal',
  standalone: true,
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.scss'],
})
export class ModalComponent implements OnInit {
  @ViewChild('modalContent', { read: ViewContainerRef, static: true })
  modalContent!: ViewContainerRef;

  constructor(
    public dialogRef: MatDialogRef<ModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { component: Type<any>, data: any }
  ) { }

  ngOnInit(): void {
    this.loadComponent();
  }

  private loadComponent(): void {
    const componentRef = this.modalContent.createComponent(this.data.component);

    if (this.data.data) {
      (componentRef.instance as NotificationComponent).data = this.data.data;
    }
  }

  closeModal(): void {
    this.dialogRef.close();
  }
}
