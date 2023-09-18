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
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { LookupItem } from 'src/app/shared/api/lookup-item';
import { ChipListComponent } from 'src/app/shared/components/chip-list/chip-list.component';

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

interface Messages {
  [chatId: string]: {
    id: string;
    senderId: string;
    senderUsername: string;
    body: string;
    date: string | Date;
    isMe: boolean;
  }[];
}

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
  templateUrl: './chat.component.html',
})
export class ChatComponent implements OnInit {
  // Inject dependencies
  private readonly _httpClientService = inject(HttpClientService);
  private readonly _destroyRef = inject(DestroyRef);

  groupChatForm = new FormGroup({
    groupName: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(3),
    ]),
    selectedUsers: new FormControl<LookupItem[]>([] as LookupItem[], [
      Validators.minLength(1),
      Validators.required,
    ]),
    userSearchInput: new FormControl<string>(''),
  });

  // UI States
  private _showMenu: WritableSignal<boolean> = signal(false);

  get showMenu() {
    return this._showMenu();
  }

  set showMenu(value: boolean) {
    this._showMenu.set(value);
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
      name: 'John Doe',
      isPrivate: true,
      lastMessage: 'Hello, how are you?',
    },
    {
      id: '2',
      name: 'John Doe',
      isPrivate: true,
      lastMessage: 'Hello, how are you?',
    },
    {
      id: '3',
      name: 'John Doe',
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
        body: 'Hello, how are you?',
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

  private _selectedUsersToChat: WritableSignal<LookupItem[]> = signal([]);

  get selectedUsersToChat() {
    return this._selectedUsersToChat();
  }

  set selectedUsersToChat(values: LookupItem[]) {
    this._selectedUsersToChat.set(values);
  }

  // Input states
  searchInput: WritableSignal<string> = signal('');
  chatMessageInput: WritableSignal<string> = signal('');

  // HTML Element references
  @ViewChild('chatMessageInputRef') private chatMessageInputRef!: ElementRef;

  @ViewChild('messagesContainerRef') private messagesContainerRef!: ElementRef;

  @ViewChild('showMenuRef') private showMenuRef!: ElementRef;

  ngOnInit() {
    // forkJoin([this._httpClientService.get({})])
    //   .pipe(takeUntilDestroyed(this._destroyRef))
    //   .subscribe();

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
    this.messagesContainerRef.nativeElement?.scrollTo({
      top: this.messagesContainerRef.nativeElement.scrollHeight,
      behavior: 'smooth',
    });
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
    const userSearchInput = this.groupChatForm.get('userSearchInput');

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
    const selectedUsers = this.groupChatForm.get('selectedUsers');

    if (selectedUsers?.value) {
      if (
        !(selectedUsers!.value as LookupItem[]).find(
          (s) => s.key === userLookup.key
        )
      ) {
        selectedUsers.setValue([...selectedUsers.value, userLookup]);
      }
    }
  }

  removeChip(chip: LookupItem) {
    this.selectedUsersToChat = this.selectedUsersToChat.filter(
      (s) => s.key !== chip.key
    );
  }

  createChat() {
    // TODO: Create chat group with API
    const formValues = this.groupChatForm.value as any as CreateChatForm;
    const newChatGroup: SidebarChatGroup = {
      name: formValues.groupName,
      isPrivate: false,
      id: '4',
      lastMessage: '',
      // selectedUsers: formValues.selectedUsers.map((s: LookupItem) => s.value),
    };

    this._sidebarChatGroups.set([...this._sidebarChatGroups(), newChatGroup]);
    // TODO: Notify users with socket

    // update sidebar chat groups
    this.createChatModalVisible = false;
    this.groupChatForm.reset();
    this.groupChatForm.get('selectedUsers')?.setValue([]);
  }
}
