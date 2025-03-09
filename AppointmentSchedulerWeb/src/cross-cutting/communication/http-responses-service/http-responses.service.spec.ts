import { TestBed } from '@angular/core/testing';

import { HttpResponsesService } from './http-responses.service';

describe('HttpResponsesService', () => {
  let service: HttpResponsesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HttpResponsesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
