import { TestBed } from '@angular/core/testing';

import { HttpClientAdapterService } from './http-client-adapter.service';

describe('HttpClientAdapterService', () => {
  let service: HttpClientAdapterService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HttpClientAdapterService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
