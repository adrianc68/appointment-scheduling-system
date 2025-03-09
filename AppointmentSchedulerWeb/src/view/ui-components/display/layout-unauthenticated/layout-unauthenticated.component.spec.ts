import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LayoutUnauthenticatedComponent } from './layout-unauthenticated.component';

describe('LayoutUnauthenticatedComponent', () => {
  let component: LayoutUnauthenticatedComponent;
  let fixture: ComponentFixture<LayoutUnauthenticatedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LayoutUnauthenticatedComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LayoutUnauthenticatedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
