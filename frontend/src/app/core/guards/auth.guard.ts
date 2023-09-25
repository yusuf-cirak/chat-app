import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { catchError, map, of, tap } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);

  if (authService.isAuthentictated) {
    return true;
  }

  const tokenService = inject(TokenService);
  const router = inject(Router);

  const accessToken = tokenService.accesToken;

  if (!accessToken) {
    return router.createUrlTree(['/login']);
  }

  return authService.refreshToken().pipe(
    map((tokenResult) => {
      tokenService.setTokens(tokenResult);

      authService.setUser(tokenService.getUserCredentialsFromToken()!);

      return true;
    }),
    catchError((_) => {
      return of(router.createUrlTree(['/login']));
    })
  );
};
