import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssistantGridItemComponent } from './assistant-grid-item.component';

describe('AssistantGridItemComponent', () => {
  let component: AssistantGridItemComponent;
  let fixture: ComponentFixture<AssistantGridItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AssistantGridItemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AssistantGridItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
