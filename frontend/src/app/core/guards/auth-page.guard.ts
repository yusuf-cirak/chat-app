import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { catchError, map, of, tap } from 'rxjs';

export const authPageGuard: CanActivateFn = (route, state) => {
  const tokenService = inject(TokenService);

  const accessToken = tokenService.accesToken;

  if (!accessToken) {
    return true;
  }

  const authService = inject(AuthService);
  const router = inject(Router);

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
