import { inject } from '@angular/core';
import {
  HttpRequest,
  HttpHandlerFn,
  HttpErrorResponse,
} from '@angular/common/http';
import { catchError, tap, throwError, mergeMap } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { TokenService } from '../services/token.service';
import { RefreshTokenResponseDto } from '../dtos/refresh-token-response-dto';

export function authInterceptor(
  request: HttpRequest<unknown>,
  next: HttpHandlerFn
) {
  // Inject dependencies
  const skipInterceptor = request.headers.get('SkipInterceptor');
  request.headers.delete('SkipInterceptor');

  if (skipInterceptor === 'true') {
    return next(request);
  }
  const authService = inject(AuthService);
  const tokenService = inject(TokenService);

  if (!authService.getUserValue()) {
    return next(request);
  }

  const router = inject(Router);
  const accessToken = tokenService.accesToken;

  const newRequest = request.clone({
    headers: request.headers.set('Authorization', `Bearer ${accessToken}`),
  });

  return next(newRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        return authService.refreshToken(true).pipe(
          catchError((_) => {
            router.navigate(['/login']);
            return throwError(() => error);
          }),
          tap((tokens: RefreshTokenResponseDto) => {
            tokenService.setTokens(tokens);
            const decodedToken = tokenService.decodeAccessToken(
              tokens.accessToken
            );
            authService.setUser(
              tokenService.getUserCredentialsFromDecodedToken(decodedToken)!
            );
          }),
          mergeMap((tokens) => {
            const newRequest = request.clone({
              headers: request.headers.set(
                'Authorization',
                `Bearer ${tokens.accessToken}`
              ),
            });
            return next(newRequest);
          })
        );
      }
      return throwError(() => error);
    })
  );
}
