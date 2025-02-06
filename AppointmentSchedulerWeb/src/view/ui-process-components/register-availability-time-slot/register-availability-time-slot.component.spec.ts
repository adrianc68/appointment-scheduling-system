import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterAvailabilityTimeSlotComponent } from './register-availability-time-slot.component';

describe('RegisterAvailabilityTimeSlotComponent', () => {
  let component: RegisterAvailabilityTimeSlotComponent;
  let fixture: ComponentFixture<RegisterAvailabilityTimeSlotComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterAvailabilityTimeSlotComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegisterAvailabilityTimeSlotComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
