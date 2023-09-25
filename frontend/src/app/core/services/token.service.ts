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
  private decodedJwt: any = {};

  get accesToken() {
    const token = localStorage.getItem('accessToken');
    if (token) {
      this.setUserCredentialsFromToken(token);
      return token;
    }
    return null;
  }

  get refreshToken() {
    return localStorage.getItem('refreshToken');
  }

  setTokens(
    tokens: RegisterResponseDto | LoginResponseDto | RefreshTokenResponseDto
  ) {
    localStorage.setItem('accessToken', tokens.accessToken);
    localStorage.setItem('refreshToken', tokens.refreshToken);

    this.setUserCredentialsFromToken(tokens.accessToken);
  }

  removeTokens() {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    this.decodedJwt = {};
  }

  setUserCredentialsFromToken(token: string) {
    for (const [key, value] of Object.entries(jwtDecode(token)!)) {
      if (key.includes('/')) {
        const newKey = key.split('/').slice(-1)[0];
        this.decodedJwt[newKey] = value;
      } else {
        this.decodedJwt[key] = value;
      }
    }
  }

  getUserCredentialsFromToken(): UserDto | null {
    if (this.decodedJwt?.nameidentifier) {
      return {
        id: this.decodedJwt?.nameidentifier,
        userName: this.decodedJwt?.unique_name,
        profilePicturePath: this.decodedJwt?.profilepicturepath,
      };
    }
    return null;
  }

  getUserIdFromToken() {
    if (!this.decodedJwt?.nameidentifier) {
      this.setUserCredentialsFromToken(this.accesToken!);
    }
    return this.decodedJwt?.nameidentifier;
  }
}
