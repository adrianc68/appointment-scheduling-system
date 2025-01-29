import { TestBed } from '@angular/core/testing';

import { TaskStateManagerService } from './task-state-manager.service';

describe('TaskStateManagerService', () => {
  let service: TaskStateManagerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TaskStateManagerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
