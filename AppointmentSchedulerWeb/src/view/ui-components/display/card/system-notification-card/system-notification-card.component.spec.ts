import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SystemNotificationCardComponent } from './system-notification-card.component';

describe('SystemNotificationCardComponent', () => {
  let component: SystemNotificationCardComponent;
  let fixture: ComponentFixture<SystemNotificationCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SystemNotificationCardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SystemNotificationCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
