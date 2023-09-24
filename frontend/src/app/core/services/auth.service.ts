import { Observable } from 'rxjs';
import { Injectable, inject } from '@angular/core';
import { HttpClientService } from 'src/app/shared/services/http-client.service';
import { RegisterUserDto } from '../dtos/register-user-dto';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly _httpClientService = inject(HttpClientService);

  register(user: RegisterUserDto) {
    return this._httpClientService.post(
      { controller: 'auth', action: 'register' },
      user
    );
  }
}
