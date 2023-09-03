import { NgClass, NgFor, NgIf } from '@angular/common';
import { Component, WritableSignal, signal } from '@angular/core';
import { InputComponent } from 'src/app/shared/components/input/input.component';

interface SidebarChatUser {
  name: string;
  lastMessage: string;
  lastMessageTime: string;
  unreadMessages: number;
}

interface ChatMessage {}

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [NgIf, NgFor, NgClass],
  templateUrl: './chat.component.html',
})
export class ChatComponent {
  selectedChatIndex: WritableSignal<number> = signal(-1);

  searchInput: WritableSignal<string> = signal('');

  chatMessageInput: WritableSignal<string> = signal('');

  sideBarChatUsers: WritableSignal<SidebarChatUser[]> = signal([]);

  chats: WritableSignal<string[]> = signal([]);

  constructor() {
    this.sideBarChatUsers.set([
      {
        name: 'John Doe',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '5 min ago',
        unreadMessages: 2,
      },
      {
        name: 'Jane Doe',
        lastMessage: 'Can we meet today?',
        lastMessageTime: '10 min ago',
        unreadMessages: 1,
      },
      {
        name: 'John Doe',
        lastMessage: "Don't forget to call me.",
        lastMessageTime: '15 min ago',
        unreadMessages: 0,
      },
      {
        name: 'Jane Doe',
        lastMessage: 'Good job!',
        lastMessageTime: '20 min ago',
        unreadMessages: 0,
      },
      {
        name: 'John Doe',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '25 min ago',
        unreadMessages: 0,
      },
      {
        name: 'Jane Doe',
        lastMessage: 'Can we meet today?',
        lastMessageTime: '30 min ago',
        unreadMessages: 0,
      },
      {
        name: 'John Doe',
        lastMessage: "Don't forget to call me.",
        lastMessageTime: '35 min ago',
        unreadMessages: 0,
      },
      {
        name: 'Jane Doe',
        lastMessage: 'Good job!',
        lastMessageTime: '40 min ago',
        unreadMessages: 0,
      },
      {
        name: 'John Doe',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '45 min ago',
        unreadMessages: 0,
      },
      {
        name: 'John Doe',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '45 min ago',
        unreadMessages: 0,
      },
      {
        name: 'John Doe',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '45 min ago',
        unreadMessages: 0,
      },
      {
        name: 'John Doe',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '45 min ago',
        unreadMessages: 0,
      },
      {
        name: 'John Doe',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '45 min ago',
        unreadMessages: 0,
      },
      {
        name: 'John Doe',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '45 min ago',
        unreadMessages: 0,
      },
      {
        name: 'Yusuf',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '45 min ago',
        unreadMessages: 0,
      },
    ]);
  }

  ngOnInit() {}

  onChatClick(index: number) {
    this.selectedChatIndex.set(index);
    this.chatMessageInput.set('');
  }

  onChatInput(value: string) {
    console.log(value);
    this.chatMessageInput.set(value);
  }
}
