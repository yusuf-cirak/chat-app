import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { catchError, map, of } from 'rxjs';

export const authPageGuard: CanActivateFn = () => {
  const router = inject(Router);

  const routeState = router.getCurrentNavigation()?.extras.state;

  if (routeState && routeState['skipGuard']) {
    return true;
  }

  const authService = inject(AuthService);

  if (authService.getUserValue()) {
    return router.createUrlTree(['/chat']);
  }

  const tokenService = inject(TokenService);
  return authService.refreshToken(true).pipe(
    map((tokenResult) => {
      tokenService.setTokensAndDecodeAccessToken(tokenResult);

      authService.setUser(tokenService.getUserCredentialsFromDecodedToken()!);

      return router.createUrlTree(['/chat']);
    }),
    catchError((_) => {
      return of(true);
    })
  );
};
