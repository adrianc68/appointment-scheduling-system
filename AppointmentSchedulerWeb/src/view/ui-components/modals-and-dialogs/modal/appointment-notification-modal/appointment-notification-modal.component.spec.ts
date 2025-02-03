import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentNotificationComponent } from './appointment-notification.component';

describe('AppointmentNotificationComponent', () => {
  let component: AppointmentNotificationComponent;
  let fixture: ComponentFixture<AppointmentNotificationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppointmentNotificationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppointmentNotificationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
