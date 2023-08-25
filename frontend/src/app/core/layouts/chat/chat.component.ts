import { NgFor, NgIf } from '@angular/common';
import { Component, WritableSignal, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

interface SidebarChatUser {
  profilePicture: string;
  name: string;
  lastMessage: string;
  lastMessageTime: string;
  unreadMessages: number;
}

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [NgIf, NgFor, FormsModule],
  templateUrl: './chat.component.html',
})
export class ChatComponent {
  sideBarChatUsers: SidebarChatUser[] = [];
  searchInput = '';

  constructor() {
    this.sideBarChatUsers = [
      {
        profilePicture: 'assets/images/noPic.svg',
        name: 'John Doe',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '5 min ago',
        unreadMessages: 2,
      },
      {
        profilePicture: 'assets/images/noPic.svg',
        name: 'Jane Doe',
        lastMessage: 'Can we meet today?',
        lastMessageTime: '10 min ago',
        unreadMessages: 1,
      },
      {
        profilePicture: 'assets/images/noPic.svg',
        name: 'John Doe',
        lastMessage: "Don't forget to call me.",
        lastMessageTime: '15 min ago',
        unreadMessages: 0,
      },
      {
        profilePicture: 'assets/images/noPic.svg',
        name: 'Jane Doe',
        lastMessage: 'Good job!',
        lastMessageTime: '20 min ago',
        unreadMessages: 0,
      },
      {
        profilePicture: 'assets/images/noPic.svg',
        name: 'John Doe',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '25 min ago',
        unreadMessages: 0,
      },
      {
        profilePicture: 'assets/images/noPic.svg',
        name: 'Jane Doe',
        lastMessage: 'Can we meet today?',
        lastMessageTime: '30 min ago',
        unreadMessages: 0,
      },
      {
        profilePicture: 'assets/images/noPic.svg',
        name: 'John Doe',
        lastMessage: "Don't forget to call me.",
        lastMessageTime: '35 min ago',
        unreadMessages: 0,
      },
      {
        profilePicture: 'assets/images/noPic.svg',
        name: 'Jane Doe',
        lastMessage: 'Good job!',
        lastMessageTime: '40 min ago',
        unreadMessages: 0,
      },
      {
        profilePicture: 'assets/images/noPic.svg',
        name: 'John Doe',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '45 min ago',
        unreadMessages: 0,
      },
      {
        profilePicture: 'assets/images/noPic.svg',
        name: 'John Doe',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '45 min ago',
        unreadMessages: 0,
      },
      {
        profilePicture: 'assets/images/noPic.svg',
        name: 'John Doe',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '45 min ago',
        unreadMessages: 0,
      },
      {
        profilePicture: 'assets/images/noPic.svg',
        name: 'John Doe',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '45 min ago',
        unreadMessages: 0,
      },
      {
        profilePicture: 'assets/images/noPic.svg',
        name: 'John Doe',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '45 min ago',
        unreadMessages: 0,
      },
      {
        profilePicture: 'assets/images/noPic.svg',
        name: 'John Doe',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '45 min ago',
        unreadMessages: 0,
      },
      {
        profilePicture: 'assets/images/noPic.svg',
        name: 'Yusuf',
        lastMessage: 'Hey, how are you?',
        lastMessageTime: '45 min ago',
        unreadMessages: 0,
      },
    ];
  }

  ngOnInit() {}
}
