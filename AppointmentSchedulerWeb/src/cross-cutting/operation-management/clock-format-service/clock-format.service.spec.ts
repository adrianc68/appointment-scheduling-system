import { TestBed } from '@angular/core/testing';

import { ClockFormatService } from './clock-format.service';

describe('ClockFormatService', () => {
  let service: ClockFormatService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ClockFormatService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
