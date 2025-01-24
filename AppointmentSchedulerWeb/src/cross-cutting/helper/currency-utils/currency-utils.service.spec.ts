import { TestBed } from '@angular/core/testing';

import { CurrencyUtilsService } from './currency-utils.service';

describe('CurrencyUtilsService', () => {
  let service: CurrencyUtilsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CurrencyUtilsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
