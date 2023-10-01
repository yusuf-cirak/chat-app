import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { catchError, map, of, take, tap } from 'rxjs';

export const authPageGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);

  const router = inject(Router);

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
