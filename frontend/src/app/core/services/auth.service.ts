import { BehaviorSubject, Observable } from 'rxjs';
import { Injectable, inject, signal } from '@angular/core';
import { HttpClientService } from 'src/app/shared/services/http-client.service';
import { RegisterUserDto } from '../dtos/register-user-dto';
import { LoginUserDto } from '../dtos/login-user-dto';
import { RefreshTokenResponseDto } from '../dtos/refresh-token-response-dto';
import { RegisterResponseDto } from '../dtos/register-response-dto';
import { LoginResponseDto } from '../dtos/login-response-dto';
import { TokenService } from './token.service';
import { HttpHeaders } from '@angular/common/http';
import { UserDto } from 'src/app/shared/api/user-dto';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly _httpClientService = inject(HttpClientService);
  private readonly _tokenService = inject(TokenService);

  private user$ = new BehaviorSubject<UserDto>(null!);

  getUserValue() {
    return this.user$.getValue();
  }

  getUserAsObservable() {
    return this.user$.asObservable();
  }

  setUser(user: UserDto) {
    this.user$.next(user);
  }

  register(user: RegisterUserDto): Observable<RegisterResponseDto> {
    return this._httpClientService.post(
      { controller: 'auth', action: 'register' },
      user
    );
  }

  login(user: LoginUserDto): Observable<LoginResponseDto> {
    return this._httpClientService.post(
      { controller: 'auth', action: 'login' },
      user
    );
  }

  refreshToken(skipInterceptor = false): Observable<RefreshTokenResponseDto> {
    const refreshToken = this._tokenService.refreshToken;
    const accessToken = this._tokenService.accesToken;
    const userId = this.getUserValue()?.id;
    return this._httpClientService.post(
      {
        controller: 'auth',
        action: 'refresh',
        headers: new HttpHeaders({
          Authorization: `Bearer ${accessToken}`,
          SkipInterceptor: skipInterceptor.valueOf().toString(),
        }),
      },
      { refreshToken, userId }
    );
  }
}
