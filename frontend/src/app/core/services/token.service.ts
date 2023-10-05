import { Injectable, WritableSignal, signal } from '@angular/core';
import { LoginResponseDto } from '../dtos/login-response-dto';
import { RefreshTokenResponseDto } from '../dtos/refresh-token-response-dto';
import { RegisterResponseDto } from '../dtos/register-response-dto';
import jwtDecode from 'jwt-decode';
import { UserDto } from 'src/app/core/dtos/user-dto';

@Injectable({
  providedIn: 'root',
})
export class TokenService {
  private _decodedToken: WritableSignal<any> = signal(null!);

  get decodedToken() {
    return this._decodedToken();
  }

  get accesToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  get refreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }

  isAccessTokenExpired(): boolean {
    if (!this.decodedToken) {
      return true;
    }

    return Date.now() >= this.decodedToken?.exp * 1000;
  }

  setTokensAndDecodeAccessToken(
    tokens: RegisterResponseDto | LoginResponseDto | RefreshTokenResponseDto
  ) {
    localStorage.setItem('accessToken', tokens.accessToken);
    localStorage.setItem('refreshToken', tokens.refreshToken);

    this.decodeAccessToken(tokens.accessToken);
  }

  removeTokens() {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }

  decodeAccessToken(token: any) {
    const decodedToken: any = {};
    for (const [key, value] of Object.entries(jwtDecode(token)!)) {
      if (key.includes('/')) {
        const newKey = key.split('/').slice(-1)[0];
        decodedToken[newKey] = value;
      } else {
        decodedToken[key] = value;
      }
    }

    if (!decodedToken['ProfileImageUrl']) {
      const profileImageUrl = localStorage.getItem('userProfileImageUrl');
      decodedToken['ProfileImageUrl'] = profileImageUrl;
    } else {
      localStorage.setItem(
        'userProfileImageUrl',
        decodedToken['ProfileImageUrl']
      );
    }

    this._decodedToken.set(decodedToken);
  }

  getUserCredentialsFromDecodedToken(): UserDto | null {
    const decodedToken = this.decodedToken;
    if (!decodedToken) {
      return null;
    }

    return {
      id: decodedToken?.nameidentifier,
      userName: decodedToken?.unique_name,
      profileImageUrl: decodedToken?.ProfileImageUrl,
    };
  }
}
