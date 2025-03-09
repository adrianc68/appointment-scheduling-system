import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentNotificationCardComponent } from './appointment-notification-card.component';

describe('AppointmentNotificationCardComponent', () => {
  let component: AppointmentNotificationCardComponent;
  let fixture: ComponentFixture<AppointmentNotificationCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppointmentNotificationCardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppointmentNotificationCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
