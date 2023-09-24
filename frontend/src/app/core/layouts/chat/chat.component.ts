import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import {
  Component,
  DestroyRef,
  ElementRef,
  HostListener,
  OnInit,
  ViewChild,
  WritableSignal,
  inject,
  signal,
} from '@angular/core';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { HttpClientService } from 'src/app/shared/services/http-client.service';
import { InputComponent } from 'src/app/shared/components/input/input.component';
import { ButtonComponent } from 'src/app/shared/components/button/button.component';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { LookupItem } from 'src/app/shared/api/lookup-item';
import { ChipListComponent } from 'src/app/shared/components/chip-list/chip-list.component';
import { ChatService } from '../../services/chat.service';
import { CreateChatGroupDto } from '../../dtos/create-chat-group-dto';
import { SendMessageDto } from '../../dtos/send-message-dto';
import { minLength } from 'src/app/shared/validators/min.length';
import { length } from 'src/app/shared/validators/length';

interface SidebarChatGroup {
  id: string;
  name: string;
  isPrivate: boolean;
  lastMessage: string;
}

interface SelectedChat {
  index: number;
  id: string;
  name: string;
  isPrivate: boolean;
}

interface ChatDateBadge {
  [chatId: string]: {
    date: Date;
    displayBeforeChatIndex: number;
  }[];
}

interface Messages {
  [chatId: string]: {
    id: string;
    senderId: string;
    senderUsername: string;
    body: string;
    date: Date;
    isMe: boolean;
  }[];
}

type ChatType = 'private' | 'group';

// Chat Group Info { id, name } - Group or user name - One API call to receive chat groups of user
// Chat Message Info { id, chatGroupId,senderId, date, message } - One API call to receive chat messages of user

// Display chat group infos at left sidebar. Show latest message of each chat group. Who send it, message, date.
// When user clicks on a chat group, display chat messages at right side. Get chat infos from chatMessageInfos

type CreateChatForm = {
  groupName: string;
  selectedUsers: LookupItem[];
  userSearchInput: string;
};

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [
    NgIf,
    NgFor,
    NgClass,
    DatePipe,
    InputComponent,
    ButtonComponent,
    ReactiveFormsModule,
    ChipListComponent,
  ],
  providers: [ChatService],
  templateUrl: './chat.component.html',
})
export class ChatComponent implements OnInit {
  // Inject dependencies
  private readonly _httpClientService = inject(HttpClientService);
  private readonly _destroyRef = inject(DestroyRef);
  private readonly _formBuilder = inject(NonNullableFormBuilder);
  private readonly chatService = inject(ChatService);

  chatForm = this._formBuilder.group({
    groupName: [
      '',
      [
        minLength(
          3,
          'Group name is required and should be at least 3 character long.'
        ),
      ],
    ],
    selectedUsers: [
      [] as LookupItem[],
      [minLength(1, 'Please select at least one user.')],
    ],
    userSearchInput: [''],
  });

  _selectedChatTypeForCreate = signal<ChatType>('private');

  // UI States
  private _showMenu: WritableSignal<boolean> = signal(false);

  get showMenu() {
    return this._showMenu();
  }

  set showMenu(value: boolean) {
    this._showMenu.set(value);
  }

  // Chat States
  private _selectedChat: WritableSignal<SelectedChat> = signal({
    index: -1,
    id: '' as any,
    name: '',
    isPrivate: false,
  });

  get selectedChat() {
    return this._selectedChat();
  }

  // Get it with http request
  private _sidebarChatGroups: WritableSignal<SidebarChatGroup[]> = signal([
    {
      id: '1',
      name: 'private1',
      isPrivate: true,
      lastMessage: 'Hello, how are you?',
    },
    {
      id: '2',
      name: 'private2',
      isPrivate: true,
      lastMessage: 'Hello, how are you?',
    },
    {
      id: '3',
      name: 'group1',
      isPrivate: false,
      lastMessage: 'Hello, how are you?',
    },
  ]);

  get sidebarChatGroups() {
    return this._sidebarChatGroups();
  }

  // Get it with http request
  private _chatMessages: WritableSignal<Messages> = signal({
    '1': [
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Hello, how are you?',
        date: new Date(),
        isMe: true,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
      {
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: 'Im fine, and you?',
        date: new Date(),
        isMe: false,
      },
    ],
    '2': [
      {
        id: '2',
        senderId: '2',
        senderUsername: 'John Doe',
        body: 'Hello, how are you?',
        date: new Date(),
        isMe: true,
      },
    ],
    '3': [
      {
        id: '3',
        senderId: '3',
        senderUsername: 'John Doe',
        body: 'Hello, how are you? ',
        date: new Date(),
        isMe: false,
      },
      {
        id: '3',
        senderId: '3',
        senderUsername: 'John Doe',
        body: 'Im fine, what about you?',
        date: new Date(),
        isMe: true,
      },
    ],
  });

  get chatMessages() {
    return this._chatMessages();
  }

  // Create chat states
  private _createChatModalVisible: WritableSignal<boolean> = signal(false);
  get createChatModalVisible() {
    return this._createChatModalVisible();
  }
  set createChatModalVisible(value: boolean) {
    this._createChatModalVisible.set(value);
  }

  private _chatBadges = signal<ChatDateBadge>({
    '1': [
      {
        date: new Date(),
        displayBeforeChatIndex: 0,
      },
    ],
    '2': [
      {
        date: new Date(),
        displayBeforeChatIndex: 0,
      },
    ],
    '3': [
      {
        date: new Date(),
        displayBeforeChatIndex: 0,
      },
    ],
  });

  get chatBadges() {
    return this._chatBadges();
  }

  // Input states
  searchInput: WritableSignal<string> = signal('');
  chatMessageInput: WritableSignal<string> = signal('');

  // HTML Element references
  @ViewChild('chatMessageInputRef')
  private chatMessageInputRef!: ElementRef;

  @ViewChild('messagesWrapperRef') private messagesWrapperRef!: ElementRef;

  @ViewChild('showMenuRef') private showMenuRef!: ElementRef;

  // Event listeners
  @HostListener('document:keyup.enter', ['$event'])
  onEnter() {
    const message = this.chatMessageInput();
    if (message.length > 0) {
      this.sendMessage(
        this.chatMessageInput(),
        this.selectedChat.id,
        this.selectedChat.index
      );
    }
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (
      !this.showMenuRef.nativeElement.contains(event.target) &&
      this.showMenu
    ) {
      this.showMenu = false;
    }
  }

  ngOnInit() {
    // forkJoin([this._httpClientService.get({})])
    //   .pipe(takeUntilDestroyed(this._destroyRef))
    //   .subscribe();

    // Register selected chat type change to update form

    // Register search input debounce
    this.registerSearchInputDebounce();
  }

  onChatClick(group: SidebarChatGroup, index: number) {
    this._selectedChat.set({
      index,
      ...group,
    });
    this.chatMessageInput.set('');
    this.chatMessageInputRef.nativeElement?.focus();
    this.scrollToBottom();
  }

  onChatInput(value: string) {
    this.chatMessageInput.set(value);
  }

  _suggestions: LookupItem[] = [
    { key: 'Ahmet', value: 1 },
    { key: 'Mehmet', value: 2 },
    { key: 'Yusuf', value: 3 },
    { key: 'Emre', value: 4 },
  ];

  suggestions: LookupItem[] = [];

  registerSearchInputDebounce() {
    const userSearchInput = this.chatForm.get('userSearchInput');

    if (userSearchInput) {
      userSearchInput.valueChanges
        .pipe(
          takeUntilDestroyed(this._destroyRef),
          debounceTime(500),
          distinctUntilChanged()
        )
        .subscribe((value) => {
          if (!value) {
            this.suggestions = [];
          } else {
            this.suggestions = this._suggestions.filter((s) =>
              s.key.toLowerCase().includes(value.toLowerCase())
            );
          }
        });
    }
  }

  onSuggestionClick(userLookup: LookupItem) {
    this.suggestions = [];
    const selectedUsers = this.chatForm.get('selectedUsers');
    const value = selectedUsers?.value;

    if (value) {
      if (!(value as LookupItem[]).find((s) => s.key === userLookup.key)) {
        selectedUsers.setValue([...selectedUsers.value, userLookup]);
      }
    }
  }

  removeChip(chip: LookupItem) {
    const values = this.chatForm.controls.selectedUsers.value as LookupItem[];
    this.chatForm.controls.selectedUsers.setValue([
      ...values.filter((s) => s.key !== chip.key),
    ]);
  }

  createChat() {
    // TODO: Create chat group with API
    const formValues = this.chatForm.value as any as CreateChatForm;

    const isChatTypePrivate = this._selectedChatTypeForCreate() === 'private';

    formValues.groupName = formValues?.groupName?.trim() || '';

    const chatObj: CreateChatGroupDto = {
      name: formValues.groupName,
      participantUserIds: formValues.selectedUsers.map(
        (s: LookupItem) => s.value as string
      ),
      isPrivate: isChatTypePrivate,
    };

    this.chatService
      .createChatGroup(chatObj)
      .pipe(takeUntilDestroyed(this._destroyRef));

    const newChatGroup: SidebarChatGroup = {
      name: isChatTypePrivate ? formValues.selectedUsers[0].key : chatObj.name,
      isPrivate: isChatTypePrivate,
      id: '4',
      lastMessage: '',
      // selectedUsers: formValues.selectedUsers.map((s: LookupItem) => s.value),
    };

    this._sidebarChatGroups.set([...this._sidebarChatGroups(), newChatGroup]);
    this._chatMessages.mutate((chatMessages) => {
      chatMessages[newChatGroup.id] = [];
    });
    // TODO: Notify users with socket

    // update sidebar chat groups
    this.createChatModalVisible = false;
    this.chatForm.reset();
  }

  sendMessage(message: string, chatId: string, index: number) {
    const messageObj: SendMessageDto = {
      content: message,
      chatGroupId: chatId,
    };

    this.chatService
      .sendMessage(messageObj)
      .pipe(takeUntilDestroyed(this._destroyRef));

    this._sidebarChatGroups.mutate((groups) => {
      groups[index].lastMessage = 'You: ' + message;
    });

    this._chatMessages.mutate((messages) => {
      messages[chatId].push({
        id: '1',
        senderId: '1',
        senderUsername: 'John Doe',
        body: message,
        date: new Date(),
        isMe: true,
      });
    });

    this.chatMessageInput.set('');

    this.scrollToBottom();
  }

  private scrollToBottom() {
    setTimeout(() => {
      const messageContainerRef =
        (this.messagesWrapperRef.nativeElement as HTMLDivElement) || null;

      if (messageContainerRef) {
        messageContainerRef.scrollTo({
          top: 10000,
          behavior: 'instant',
        });
      }
    }, 0);
  }

  openCreateChatModal(chatType: ChatType) {
    this._selectedChatTypeForCreate.set(chatType);
    this.updateForm(chatType);
    this.createChatModalVisible = true;
  }

  updateForm(chatType: ChatType) {
    if (chatType === 'private') {
      this.chatForm.controls.groupName.setValidators(null);

      this.chatForm.controls.groupName.disable();

      this.chatForm.controls.selectedUsers.setValidators(
        length(1, 'You must select a user to chat.')
      );

      // Update validation of controls
      this.chatForm.controls.groupName.updateValueAndValidity();

      this.chatForm.controls.selectedUsers.updateValueAndValidity();
    } else {
      this.chatForm.controls.groupName.enable();

      this.chatForm.controls.groupName.setValidators([
        minLength(
          3,
          'Group name is required and should be at least 3 character long.'
        ),
      ]);

      this.chatForm.controls.selectedUsers.setValidators(
        minLength(1, 'You must select at least one user for your chat group.')
      );

      // Update validation of controls

      this.chatForm.controls.groupName.updateValueAndValidity();

      this.chatForm.controls.selectedUsers.updateValueAndValidity();
    }
  }

  displayChatDateBadge(date: Date): string {
    const today = new Date().getDate();
    if (today === date.getDate()) {
      return 'TODAY';
    } else if (today - date.getDate() === 1) {
      return 'YESTERDAY';
    }
    return date.getMonth() + 1 + '/' + date.getDate();
  }
}
