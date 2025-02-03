import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GeneralNotificationComponent } from './general-notification.component';

describe('GeneralNotificationComponent', () => {
  let component: GeneralNotificationComponent;
  let fixture: ComponentFixture<GeneralNotificationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GeneralNotificationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GeneralNotificationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
