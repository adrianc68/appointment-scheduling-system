import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LandingPageHeaderComponent } from './landing-page-header.component';

describe('LandingPageHeaderComponent', () => {
  let component: LandingPageHeaderComponent;
  let fixture: ComponentFixture<LandingPageHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LandingPageHeaderComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LandingPageHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
