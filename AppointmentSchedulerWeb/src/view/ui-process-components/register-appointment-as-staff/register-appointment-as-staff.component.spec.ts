import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterAppointmentAsStaffComponent } from './register-appointment-as-staff.component';

describe('RegisterAppointmentAsStaffComponent', () => {
  let component: RegisterAppointmentAsStaffComponent;
  let fixture: ComponentFixture<RegisterAppointmentAsStaffComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterAppointmentAsStaffComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegisterAppointmentAsStaffComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
