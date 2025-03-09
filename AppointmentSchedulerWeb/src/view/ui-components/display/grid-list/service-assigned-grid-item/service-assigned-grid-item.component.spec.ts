import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceAssignedGridItemComponent } from './service-assigned-grid-item.component';

describe('ServiceAssignedGridItemComponent', () => {
  let component: ServiceAssignedGridItemComponent;
  let fixture: ComponentFixture<ServiceAssignedGridItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ServiceAssignedGridItemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ServiceAssignedGridItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
