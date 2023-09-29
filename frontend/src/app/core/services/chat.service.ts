import { Injectable, inject } from '@angular/core';
import { HttpClientService } from 'src/app/shared/services/http-client.service';
import { CreateChatGroupDto } from '../dtos/create-chat-group-dto';
import { SendMessageDto } from '../dtos/send-message-dto';
import { Observable } from 'rxjs';
import { UserDto } from 'src/app/shared/api/user-dto';
import { ChatGroupDto } from '../dtos/chat-group-dto';
import { ChatGroupMessagesDto } from '../dtos/chat-group-messages-dto';

@Injectable({ providedIn: 'root' })
export class ChatService {
  private readonly _httpClientService = inject(HttpClientService);

  getChatUsers(): Observable<UserDto[]> {
    return this._httpClientService.get({
      controller: 'users',
      action: 'chat-users',
    });
  }

  getChatGroups(): Observable<ChatGroupDto[]> {
    return this._httpClientService.get({ controller: 'chatGroups' });
  }

  getChatGroupMessages(): Observable<ChatGroupMessagesDto> {
    return this._httpClientService.get({ controller: 'messages' });
  }

  createChatGroup(chatObj: CreateChatGroupDto): Observable<string> {
    return this._httpClientService.post({ controller: 'chatGroups' }, chatObj);
  }

  sendMessage(messageObj: SendMessageDto): Observable<string> {
    return this._httpClientService.post({ controller: 'messages' }, messageObj);
  }
}
