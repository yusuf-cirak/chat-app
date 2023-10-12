import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { catchError, map, of } from 'rxjs';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);

  if (authService.getUserValue()) {
    return true;
  }

  const router = inject(Router);

  const tokenService = inject(TokenService);

  const accessToken = tokenService.accesToken;

  if (!accessToken) {
    return router.createUrlTree(['/login']);
  }

  return authService.refreshToken(true).pipe(
    map((tokenResult) => {
      tokenService.setTokensAndDecodeAccessToken(tokenResult);

      authService.setUser(tokenService.getUserCredentialsFromDecodedToken()!);

      return true;
    }),
    catchError((_) => {
      return of(router.createUrlTree(['/login']));
    })
  );
};
