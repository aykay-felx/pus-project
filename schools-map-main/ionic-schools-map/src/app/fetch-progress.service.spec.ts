import { TestBed } from '@angular/core/testing';

import { FetchProgressService } from './fetch-progress.service';

describe('FetchProgressService', () => {
  let service: FetchProgressService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FetchProgressService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
