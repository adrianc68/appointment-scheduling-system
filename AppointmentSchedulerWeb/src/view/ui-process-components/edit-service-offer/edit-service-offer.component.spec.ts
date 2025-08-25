import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditServiceOfferComponent } from './edit-service-offer.component';

describe('EditServiceOfferComponent', () => {
  let component: EditServiceOfferComponent;
  let fixture: ComponentFixture<EditServiceOfferComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditServiceOfferComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditServiceOfferComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
