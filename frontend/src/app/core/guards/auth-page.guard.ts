import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { catchError, map, of, tap } from 'rxjs';

export const authPageGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);

  const router = inject(Router);

  if (authService.getUserValue()) {
    return router.createUrlTree(['/chat']);
  }

  const tokenService = inject(TokenService);

  const accessToken = tokenService.accesToken;

  if (!accessToken) {
    return true;
  }

  if (!tokenService.isAccessTokenExpired()) {
    return router.createUrlTree(['/chat']);
  }

  return authService.refreshToken().pipe(
    map((tokenResult) => {
      tokenService.setTokens(tokenResult);

      authService.setUser(tokenService.getUserCredentialsFromToken()!);

      return router.createUrlTree(['/chat']);
    }),
    catchError((_) => {
      return of(true);
    })
  );
};
