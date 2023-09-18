import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { HttpClientService } from 'src/app/shared/services/http-client.service';

@Injectable()
export class ChatService {
  private readonly _httpClientService = inject(HttpClientService);

  getChats() {
    return this._httpClientService.get({});
  }
}
