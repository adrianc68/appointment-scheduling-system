import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GeneralNotificationCardComponent } from './general-notification-card.component';

describe('GeneralNotificationCardComponent', () => {
  let component: GeneralNotificationCardComponent;
  let fixture: ComponentFixture<GeneralNotificationCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GeneralNotificationCardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GeneralNotificationCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
