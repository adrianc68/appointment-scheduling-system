import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterServiceOfferComponent } from './register-service-offer.component';

describe('RegisterServiceOfferComponent', () => {
  let component: RegisterServiceOfferComponent;
  let fixture: ComponentFixture<RegisterServiceOfferComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterServiceOfferComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegisterServiceOfferComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
