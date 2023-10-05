import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { MessageDto } from '../dtos/chat-group-messages-dto';
import { ChatGroupDto } from '../dtos/chat-group-dto';

@Injectable({
  providedIn: 'root',
})
export class ChatHub {
  private readonly _hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(environment.chatHubUrl!, {
      accessTokenFactory: () => localStorage.getItem('accessToken')!,
    })
    .build();

  chatMessageReceived$ = new BehaviorSubject<MessageDto>(null!);
  chatGroupCreated$ = new BehaviorSubject<ChatGroupDto>(null!);

  connectToChatHub() {
    this._hubConnection
      .start()
      .then(() => {
        console.log('Connected to chat hub');
      })
      .catch((err) => {
        console.error(err);
      });
  }

  async disconnectFromHub() {
    await this._hubConnection.stop();
  }

  registerChatHubHandlers() {
    this._hubConnection.on('ReceiveMessageAsync', (message: MessageDto) => {
      this.chatMessageReceived$.next(message);
    });

    this._hubConnection.on(
      'ChatGroupCreatedAsync',
      (chatGroup: ChatGroupDto) => {
        this.chatGroupCreated$.next(chatGroup);
      }
    );
  }

  invokeMessageSend(message: MessageDto, recipientUserIds: string[]) {
    this._hubConnection
      .invoke('SendMessageAsync', message, recipientUserIds)
      .then(() => {
        console.log('Message sent');
      })
      .catch((err) => {
        console.error(err);
      });
  }

  invokeChatGroupCreated(chatGroup: ChatGroupDto) {
    this._hubConnection
      .invoke('CreateChatGroupAsync', chatGroup)
      .then(() => {
        console.log('Chat group created');
      })
      .catch((err) => {
        console.error(err);
      });
  }
}
