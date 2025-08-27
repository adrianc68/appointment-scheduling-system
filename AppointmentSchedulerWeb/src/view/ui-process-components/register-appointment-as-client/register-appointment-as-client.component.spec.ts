import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterAppointmentAsClientComponent } from './register-appointment-as-client.component';

describe('RegisterAppointmentAsClientComponent', () => {
  let component: RegisterAppointmentAsClientComponent;
  let fixture: ComponentFixture<RegisterAppointmentAsClientComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterAppointmentAsClientComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegisterAppointmentAsClientComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
