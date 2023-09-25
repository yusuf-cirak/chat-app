import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { authPageGuard } from './auth-page.guard';

describe('authPageGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => authPageGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
