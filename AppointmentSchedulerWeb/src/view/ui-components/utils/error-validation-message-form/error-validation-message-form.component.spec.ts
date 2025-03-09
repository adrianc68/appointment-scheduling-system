import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ErrorValidationMessageFormComponent } from './error-validation-message-form.component';

describe('ErrorValidationMessageFormComponent', () => {
  let component: ErrorValidationMessageFormComponent;
  let fixture: ComponentFixture<ErrorValidationMessageFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ErrorValidationMessageFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ErrorValidationMessageFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
