import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceOfferManagementComponent } from './service-offer-management.component';

describe('ServiceOfferManagementComponent', () => {
  let component: ServiceOfferManagementComponent;
  let fixture: ComponentFixture<ServiceOfferManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ServiceOfferManagementComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ServiceOfferManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
