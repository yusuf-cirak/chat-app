import { Injectable, inject } from '@angular/core';
import { HttpClientService } from 'src/app/shared/services/http-client.service';
import { CreateChatGroupDto } from '../dtos/create-chat-group-dto';
import { SendMessageDto } from '../dtos/send-message-dto';

@Injectable()
export class ChatService {
  private readonly _httpClientService = inject(HttpClientService);

  getChats() {
    return this._httpClientService.get({});
  }

  createChatGroup(chatObj: CreateChatGroupDto) {
    return this._httpClientService.post({}, chatObj);
  }

  sendMessage(messageObj: SendMessageDto) {
    return this._httpClientService.post({}, messageObj);
  }
}
