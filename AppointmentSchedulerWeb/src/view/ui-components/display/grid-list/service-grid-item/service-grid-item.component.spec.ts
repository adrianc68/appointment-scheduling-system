import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceGridItemComponent } from './service-grid-item.component';

describe('ServiceGridItemComponent', () => {
  let component: ServiceGridItemComponent;
  let fixture: ComponentFixture<ServiceGridItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ServiceGridItemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ServiceGridItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
