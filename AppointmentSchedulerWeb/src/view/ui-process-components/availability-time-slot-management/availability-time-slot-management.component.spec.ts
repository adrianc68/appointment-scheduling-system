import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AvailabilityTimeSlotManagementComponent } from './availability-time-slot-management.component';

describe('AvailabilityTimeSlotManagementComponent', () => {
  let component: AvailabilityTimeSlotManagementComponent;
  let fixture: ComponentFixture<AvailabilityTimeSlotManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AvailabilityTimeSlotManagementComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AvailabilityTimeSlotManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
