import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ButtonTaskComponent } from './button-task.component';

describe('ButtonTaskComponent', () => {
  let component: ButtonTaskComponent;
  let fixture: ComponentFixture<ButtonTaskComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ButtonTaskComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ButtonTaskComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
