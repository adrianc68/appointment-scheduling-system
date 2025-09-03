import { TestBed } from '@angular/core/testing';

import { ErrorUIService } from './error-ui.service';

describe('ErrorUIService', () => {
  let service: ErrorUIService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ErrorUIService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
