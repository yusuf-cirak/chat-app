import { Injectable, inject } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, catchError, tap, throwError, mergeMap } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { TokenService } from '../services/token.service';

@Injectable()
export class HttpRequestInterceptor implements HttpInterceptor {
  // Inject dependencies
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly tokenService = inject(TokenService);

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    const token = localStorage.getItem('accessToken');
    const newRequest = request.clone({
      headers: request.headers.set('Authorization', `Bearer ${token}`),
    });
    return next.handle(newRequest).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          return this.authService.refreshToken().pipe(
            catchError((error: HttpErrorResponse) => {
              this.router.navigateByUrl('/login');
              return throwError(() => error.error);
            }),
            tap((tokenResult) => {
              this.tokenService.setTokens(tokenResult);
            }),
            mergeMap((tokenResult) => {
              const newRequest = request.clone({
                headers: request.headers.set(
                  'Authorization',
                  `Bearer ${tokenResult.accessToken}`
                ),
              });
              return next.handle(newRequest);
            })
          );
        }
        return throwError(() => error.error);
      })
    );
  }
}
