import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssistantManagementComponent } from './assistant-management.component';

describe('AssistantManagementComponent', () => {
  let component: AssistantManagementComponent;
  let fixture: ComponentFixture<AssistantManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AssistantManagementComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AssistantManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
