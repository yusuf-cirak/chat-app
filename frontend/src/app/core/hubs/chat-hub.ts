import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { MessageDto } from '../dtos/chat-group-messages-dto';
import { ChatGroupDto } from '../dtos/chat-group-dto';
import { CreateHubChatGroupDto } from '../dtos/create-hub-chat-group-dto';

@Injectable()
export class ChatHub {
  private readonly _hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(environment.chatHubUrl!, {
      accessTokenFactory: () => localStorage.getItem('accessToken')!,
    })
    .build();

  chatMessageReceived$ = new BehaviorSubject<MessageDto>(null!);
  chatGroupCreated$ = new BehaviorSubject<CreateHubChatGroupDto>(null!);

  connectToChatHub() {
    this._hubConnection
      .start()
      .then(() => {
        console.log('Connected to chat hub');
        this.registerChatHubHandlers();
        console.log('Registered chat hub handlers');
      })
      .catch((err) => {
        console.error(err);
      });
  }

  disconnectFromHub() {
    this._hubConnection
      .stop()
      .then(() => {
        console.log('Disconnected from chat hub');
      })
      .catch((err) => {
        console.error(err);
      });
  }

  registerChatHubHandlers() {
    this._hubConnection.on('ReceiveMessageAsync', (message: MessageDto) => {
      this.chatMessageReceived$.next(message);
    });

    this._hubConnection.on(
      'ChatGroupCreatedAsync',
      (chatGroup: CreateHubChatGroupDto) => {
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

  invokeChatGroupCreated(newChatGroup: CreateHubChatGroupDto) {
    this._hubConnection
      .invoke('CreateChatGroupAsync', newChatGroup)
      .then(() => {
        console.log('Chat group created');
      })
      .catch((err) => {
        console.error(err);
      });
  }
}
