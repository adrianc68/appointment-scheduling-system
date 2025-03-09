import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditAvailabilityTimeSlotComponent } from './edit-availability-time-slot.component';

describe('EditAvailabilityTimeSlotComponent', () => {
  let component: EditAvailabilityTimeSlotComponent;
  let fixture: ComponentFixture<EditAvailabilityTimeSlotComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditAvailabilityTimeSlotComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditAvailabilityTimeSlotComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
