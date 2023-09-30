import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { catchError, map, of, take, tap } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);

  if (authService.getUserValue()) {
    return true;
  }

  const tokenService = inject(TokenService);

  const router = inject(Router);

  const accessToken = tokenService.accesToken;

  if (!accessToken) {
    return router.createUrlTree(['/login']);
  }

  return authService.refreshToken(true).pipe(
    map((tokenResult) => {
      tokenService.setTokens(tokenResult);

      const decodedToken = tokenService.decodeAccessToken(accessToken);

      authService.setUser(
        tokenService.getUserCredentialsFromDecodedToken(decodedToken)!
      );

      return true;
    }),
    catchError((_) => {
      return of(router.createUrlTree(['/login']));
    })
  );
};