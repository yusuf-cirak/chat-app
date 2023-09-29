import { Injectable } from '@angular/core';
import { LoginResponseDto } from '../dtos/login-response-dto';
import { RefreshTokenResponseDto } from '../dtos/refresh-token-response-dto';
import { RegisterResponseDto } from '../dtos/register-response-dto';
import jwtDecode from 'jwt-decode';
import { UserDto } from 'src/app/shared/api/user-dto';

@Injectable({
  providedIn: 'root',
})
export class TokenService {
  get accesToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  isTokenExpired(decodedToken: any): boolean {
    if (!decodedToken) {
      return true;
    }

    return Date.now() >= decodedToken.exp * 1000;
  }

  get refreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }

  setTokens(
    tokens: RegisterResponseDto | LoginResponseDto | RefreshTokenResponseDto
  ) {
    localStorage.setItem('accessToken', tokens.accessToken);
    localStorage.setItem('refreshToken', tokens.refreshToken);
  }

  removeTokens() {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }

  decodeAccessToken(token: any) {
    if (!token) {
      return null;
    }
    const decodedToken: any = {};
    for (const [key, value] of Object.entries(jwtDecode(token)!)) {
      if (key.includes('/')) {
        const newKey = key.split('/').slice(-1)[0];
        decodedToken[newKey] = value;
      } else {
        decodedToken[key] = value;
      }
    }

    return decodedToken;
  }

  getUserCredentialsFromDecodedToken(decodedToken: any): UserDto | null {
    if (!decodedToken) {
      return null;
    }

    return {
      id: decodedToken?.nameidentifier,
      userName: decodedToken?.unique_name,
      profilePicturePath: decodedToken?.profilepicturepath,
    };
  }
}
