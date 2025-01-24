import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StateIndicatorComponent } from './state-indicator.component';

describe('StateIndicatorComponent', () => {
  let component: StateIndicatorComponent;
  let fixture: ComponentFixture<StateIndicatorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StateIndicatorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StateIndicatorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
