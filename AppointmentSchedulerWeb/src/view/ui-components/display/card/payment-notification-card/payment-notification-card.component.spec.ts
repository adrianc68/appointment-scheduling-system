import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentNotificationCardComponent } from './payment-notification-card.component';

describe('PaymentNotificationCardComponent', () => {
  let component: PaymentNotificationCardComponent;
  let fixture: ComponentFixture<PaymentNotificationCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaymentNotificationCardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PaymentNotificationCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
