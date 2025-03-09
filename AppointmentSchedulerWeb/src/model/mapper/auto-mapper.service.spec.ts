import { TestBed } from '@angular/core/testing';

import { AutoMapperService } from './auto-mapper.service';

describe('AutoMapperService', () => {
  let service: AutoMapperService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AutoMapperService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
