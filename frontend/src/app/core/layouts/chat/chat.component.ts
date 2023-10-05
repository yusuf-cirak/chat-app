import { ImageService } from './../../services/image.service';
import { MessageDto } from './../../dtos/chat-group-messages-dto';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AsyncPipe, DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
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
import {
  debounceTime,
  distinctUntilChanged,
  filter,
  forkJoin,
  of,
  switchMap,
} from 'rxjs';
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
import { Router } from '@angular/router';
import { TokenService } from '../../services/token.service';
import { AuthService } from '../../services/auth.service';
import { UserDto } from 'src/app/core/dtos/user-dto';
import { ChatGroupDto } from '../../dtos/chat-group-dto';
import { ToastrService } from 'ngx-toastr';
import { ChatHub } from '../../hubs/chat-hub';
import { ListSkeletonComponent } from 'src/app/shared/components/skelenots/list/list-skeleton.component';

interface SidebarChatGroup {
  id: string;
  name: string;
  userIds: string[];
  isPrivate: boolean;
  lastMessage: string;
  profileImageUrl: string;
}

interface SelectedChat {
  index: number;
  id: string;
  name: string;
  userIds: string[];
  isPrivate: boolean;
  profileImageUrl: string;
}

interface ChatDateBadge {
  [chatId: string]: {
    date: Date;
    displayBeforeChatIndex: number;
  }[];
}

interface ChatMessage {
  id: string;
  senderId: string;
  body: string;
  date: Date;
  isMe: boolean;
}
interface Messages {
  [chatId: string]: ChatMessage[];
}

interface ChatUserDictionary {
  [userId: string]: {
    userName: string;
    profileImageUrl: string;
  };
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
    AsyncPipe,
    DatePipe,
    InputComponent,
    ButtonComponent,
    ReactiveFormsModule,
    ChipListComponent,
    ListSkeletonComponent,
  ],
  templateUrl: './chat.component.html',
})
export class ChatComponent implements OnInit {
  // Inject dependencies
  private readonly _router = inject(Router);
  private readonly _destroyRef = inject(DestroyRef);
  private readonly _formBuilder = inject(NonNullableFormBuilder);
  private readonly chatService = inject(ChatService);
  private readonly tokenService = inject(TokenService);
  private readonly authService = inject(AuthService);
  private readonly imageService = inject(ImageService);
  private readonly toastrService = inject(ToastrService);

  private readonly chatHub = inject(ChatHub);

  currentUser$ = this.authService.getUserAsObservable();
  get currentUserId() {
    return this.authService.getUserValue().id;
  }

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

  // Create chat states
  private _createChatModalVisible: WritableSignal<boolean> = signal(false);
  get createChatModalVisible() {
    return this._createChatModalVisible();
  }
  set createChatModalVisible(value: boolean) {
    this._createChatModalVisible.set(value);
  }
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
    userIds: [],
    isPrivate: false,
    profileImageUrl: '',
  });

  get selectedChat() {
    return this._selectedChat();
  }

  private _chatUsers: WritableSignal<ChatUserDictionary> = signal({});

  get chatUsers() {
    return this._chatUsers();
  }

  // Get it with http request
  private _sidebarChatGroups: WritableSignal<SidebarChatGroup[]> = signal([]);
  private _filteredChatGroups: WritableSignal<SidebarChatGroup[]> = signal([]);

  get sidebarChatGroupsLength() {
    return this._sidebarChatGroups().length;
  }
  get filteredChatGroups() {
    return this._filteredChatGroups();
  }

  // Get it with http request
  private _chatMessages: WritableSignal<Messages> = signal({});

  get chatMessages() {
    return this._chatMessages();
  }

  private _chatBadges = signal<ChatDateBadge>({});

  get chatBadges() {
    return this._chatBadges();
  }

  _suggestions: WritableSignal<LookupItem[]> = signal([]);

  get suggestions() {
    return this._suggestions();
  }

  private today = new Date();
  // Set yesterday onInit
  private yesterday = new Date();

  // Input states
  chatMessageInput: WritableSignal<string> = signal('');

  // HTML Element references
  @ViewChild('chatMessageInputRef')
  private chatMessageInputRef!: ElementRef;

  @ViewChild('messagesWrapperRef') private messagesWrapperRef!: ElementRef;

  @ViewChild('showMenuRef') private showMenuRef!: ElementRef;

  @ViewChild('createChatModalRef') private createChatModalRef!: ElementRef;

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
      this.showMenu &&
      !this.showMenuRef.nativeElement.contains(event.target)
    ) {
      this.showMenu = false;
    }
  }

  ngOnInit() {
    this.yesterday.setDate(this.today.getDate() - 1);

    forkJoin([
      this.chatService.getChatUsers(),
      this.chatService.getChatGroups(),
      this.chatService.getChatGroupMessages(),
    ]).subscribe({
      next: ([users, groups, messages]) => {
        // Set all the chat users => _chatUsers
        // ChatUsers is an dictionary object with key as userId and value as ChatUser, but the "users" payload from API is an array of ChatUser
        const chatUsers: ChatUserDictionary = users.reduce(
          (chatUsersObj: ChatUserDictionary, currentUser: UserDto) => {
            chatUsersObj[currentUser.id] = {
              userName: currentUser.userName,
              profileImageUrl: currentUser.profileImageUrl,
            };
            return chatUsersObj;
          },
          {}
        );
        this._chatUsers.set(chatUsers);

        // Set all the chat messages _chatMessages
        // "messages" payload from API lacks the isMe property, so we need to add it manually
        // We also set the chat badges here, no need for another loop
        const chatMessages: Messages = {};
        const chatBadges: ChatDateBadge = {};
        for (const [chatId, values] of Object.entries(messages)) {
          chatMessages[chatId] = !values
            ? []
            : values.reduce(
                (
                  messagesArr: ChatMessage[],
                  current: MessageDto,
                  currentIndex: number
                ) => {
                  const messageSentDate = new Date(current.sentAt);
                  if (typeof chatBadges[chatId] === 'undefined') {
                    chatBadges[chatId] = [];
                  }
                  if (
                    !chatBadges[chatId].some(
                      (chatBadge) =>
                        chatBadge.date.getDate() ===
                          messageSentDate.getDate() &&
                        chatBadge.date.getMonth() ===
                          messageSentDate.getMonth() &&
                        chatBadge.date.getFullYear() ===
                          messageSentDate.getFullYear()
                    )
                  ) {
                    chatBadges[chatId].push({
                      date: messageSentDate,
                      displayBeforeChatIndex: currentIndex,
                    });
                  }

                  messagesArr.push({
                    body: current.body,
                    date: messageSentDate,
                    id: current.id,
                    isMe: current.userId === this.currentUserId,
                    senderId: current.userId,
                  });
                  return messagesArr;
                },
                []
              );
        }

        this._chatMessages.set(chatMessages);
        this._chatBadges.set(chatBadges);

        // Set all the sidebar chat groups _sidebarChatGroups
        // "groups" payload from API lacks the lastMessage property, so we need to add it manually
        const sidebarChatGroups: SidebarChatGroup[] = groups.map((group) => {
          const isChatGroupPrivate = group.isPrivate;
          const lastMessageObj = chatMessages[group.id]?.slice(-1)[0];
          let lastMessage = '';
          let groupName = group.name;
          let chatGroupImageUrl = '';
          if (isChatGroupPrivate) {
            lastMessage = lastMessageObj?.body;
            const otherUserId = group.userIds.find(
              (id) => id !== this.currentUserId
            );
            const otherUser = chatUsers[otherUserId!];
            groupName = otherUser.userName;
            chatGroupImageUrl = otherUser.profileImageUrl;
          } else {
            if (lastMessageObj) {
              lastMessage = lastMessageObj.isMe
                ? `You: ${lastMessageObj.body}`
                : `${chatUsers[lastMessageObj?.senderId]?.userName}: ${
                    lastMessageObj?.body
                  }`;
            }
          }
          return {
            id: group.id,
            name: groupName,
            isPrivate: group.isPrivate,
            userIds: group.userIds,
            lastMessage: lastMessage,
            profileImageUrl: chatGroupImageUrl,
          };
        });

        this._sidebarChatGroups.set(sidebarChatGroups);
        this._filteredChatGroups.set(sidebarChatGroups);
      },
    });

    // Register selected chat type change to update form

    // Register search input debounce
    this.registerSearchInputDebounce();

    // Connect to hub

    this.chatHub.connectToChatHub();
    this.chatHub.registerChatHubHandlers();

    this.chatHub.chatMessageReceived$
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (message) => {
          if (!message) {
            return;
          }
          const chatGroupId = message.chatGroupId;
          const chatGroupIndex = this._sidebarChatGroups().findIndex(
            (scg) => scg.id === chatGroupId
          );
          if (chatGroupIndex === -1) {
            return;
          }
          this._sidebarChatGroups.mutate((groups) => {
            groups[chatGroupIndex].lastMessage = message.body;
          });
          this._chatMessages.mutate((messages) => {
            if (messages[chatGroupId] === undefined) {
              messages[chatGroupId] = [];
            }
            messages[chatGroupId].push({
              id: message.id,
              senderId: message.userId,
              body: message.body,
              date: new Date(message.sentAt),
              isMe: false,
            });
          });
          this.scrollToBottom();
        },
      });

    this.chatHub.chatGroupCreated$
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (chatGroup) => {
          if (!chatGroup) {
            return;
          }
          const newChatGroup: SidebarChatGroup = {
            name: chatGroup.name,
            userIds: chatGroup.userIds,
            isPrivate: chatGroup.isPrivate,
            id: chatGroup.id,
            lastMessage: '',
            profileImageUrl: '',
          };

          this._sidebarChatGroups.set([
            ...this._sidebarChatGroups(),
            newChatGroup,
          ]);

          if (
            this._sidebarChatGroups().length ===
            this._filteredChatGroups().length
          ) {
            this._filteredChatGroups.set([
              ...this._filteredChatGroups(),
              newChatGroup,
            ]);
          }
          this._chatMessages.mutate((chatMessages) => {
            chatMessages[newChatGroup.id] = [];
          });

          this._selectedChat.set({
            ...newChatGroup,
            index: this._sidebarChatGroups().length - 1,
          });
        },
      });
  }

  onChatClick(group: SidebarChatGroup, index: number) {
    this._selectedChat.set({
      index,
      ...group,
    });
    this.chatMessageInput.set('');
    this.focusChatInput();
    this.scrollToBottom();
  }

  onChatInput(value: string) {
    this.chatMessageInput.set(value);
  }

  registerSearchInputDebounce() {
    const userSearchInput = this.chatForm.get('userSearchInput');

    if (userSearchInput) {
      userSearchInput.valueChanges
        .pipe(
          takeUntilDestroyed(this._destroyRef),
          debounceTime(200),
          distinctUntilChanged(),
          filter((searchText) => {
            if (searchText.length === 0) {
              this._suggestions.set([]);
              return false;
            }
            return true;
          }),
          switchMap((searchText) => this.chatService.searchUsers(searchText))
        )
        .subscribe((usersResult: UserDto[]) => {
          if (!usersResult.length) {
            this._suggestions.set([]);
          } else {
            const suggestions = usersResult.map((user) => ({
              key: user.userName,
              value: user,
            }));
            this._suggestions.set(suggestions);
            // usersResult.forEach((user) => {
            //   this.chatUsers[user.id] = {
            //     userName: user.userName,
            //     profileImageUrl: user.profileImageUrl,
            //   };
            // });
          }
        });
    }
  }

  onSuggestionClick(userLookup: LookupItem) {
    this._suggestions.set([]);
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
        (s: LookupItem) => (s.value as UserDto).id
      ),
      isPrivate: isChatTypePrivate,
    };

    chatObj.participantUserIds.push(this.currentUserId);

    this.chatService
      .createChatGroup(chatObj)
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (chatGroupId) => {
          const chatGroupIndex = this._sidebarChatGroups().findIndex(
            (scg) => scg.id === chatGroupId
          );
          // Chat group already exists
          if (chatGroupIndex !== -1) {
            this._selectedChat.set({
              ...this._sidebarChatGroups()[chatGroupIndex],
              index: chatGroupIndex,
            });
            this.createChatModalVisible = false;
            this.chatForm.reset();
            return;
          }
          const newChatGroup: SidebarChatGroup = {
            name: isChatTypePrivate
              ? formValues.selectedUsers[0].key
              : chatObj.name,
            userIds: chatObj.participantUserIds,
            isPrivate: isChatTypePrivate,
            id: chatGroupId,
            lastMessage: '',
            profileImageUrl: '',
          };

          this._sidebarChatGroups.set([
            ...this._sidebarChatGroups(),
            newChatGroup,
          ]);

          if (
            this._sidebarChatGroups().length ===
            this._filteredChatGroups().length
          ) {
            this._filteredChatGroups.set([
              ...this._filteredChatGroups(),
              newChatGroup,
            ]);
          }
          this._chatMessages.mutate((chatMessages) => {
            chatMessages[newChatGroup.id] = [];
          });

          this._selectedChat.set({
            ...newChatGroup,
            index: this._sidebarChatGroups().length - 1,
          });

          this.createChatModalVisible = false;
          this.chatForm.reset();
        },
      });
  }

  sendMessage(message: string, chatId: string, index: number) {
    if (!message.length) {
      return;
    }
    const messageObj: SendMessageDto = {
      content: message,
      chatGroupId: chatId,
    };

    this.chatService
      .sendMessage(messageObj)
      .pipe(takeUntilDestroyed(this._destroyRef))
      .subscribe({
        next: (messageId) => {
          const messageDto: MessageDto = {
            id: messageId,
            userId: this.currentUserId,
            chatGroupId: chatId,
            body: message,
            sentAt: new Date(),
          };
          const recipientUserIds = this._sidebarChatGroups()[
            index
          ].userIds.filter((id) => id !== this.currentUserId);

          // Send message to hub, server will handle the rest
          this.chatHub.sendMessage(messageDto, recipientUserIds);

          this._sidebarChatGroups.mutate((groups) => {
            groups[index].lastMessage = groups[index].isPrivate
              ? message
              : `You: ${message}`;
          });

          this._chatMessages.mutate((messages) => {
            if (messages[chatId] === undefined) {
              messages[chatId] = [];
            }
            messages[chatId].push({
              id: messageId,
              senderId: this.currentUserId,
              body: message,
              date: new Date(),
              isMe: true,
            });
          });

          this.chatMessageInput.set('');

          this.scrollToBottom();
        },
      });
  }

  private scrollToBottom() {
    setTimeout(() => {
      const messageContainerRef =
        (this.messagesWrapperRef.nativeElement as HTMLDivElement) || null;

      if (messageContainerRef) {
        messageContainerRef.scrollTo({
          top: messageContainerRef.scrollHeight,
          behavior: 'instant',
        });
      }
    }, 0);
  }

  private focusChatInput() {
    setTimeout(() => {
      const chatInputRef =
        (this.chatMessageInputRef.nativeElement as HTMLDivElement) || null;

      if (chatInputRef) {
        chatInputRef.focus();
      }
    }, 0);
  }

  getPrivateChatDisplayName(
    group: SidebarChatGroup | SelectedChat | ChatGroupDto
  ): string {
    const otherUserId = group.userIds.find((u) => u !== this.currentUserId)!;
    return this.chatUsers[otherUserId].userName;
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

  logout() {
    this.tokenService.removeTokens();
    this.authService.setUser(null!);
    this._router.navigate(['/login']);
  }

  filterChatGroups(searchInput: string) {
    if (searchInput.length) {
      const filteredChatGroups = this._sidebarChatGroups().filter((group) =>
        group.name.toLowerCase().includes(searchInput.toLowerCase())
      );
      this._filteredChatGroups.set(filteredChatGroups);
    } else {
      this._filteredChatGroups.set(this._sidebarChatGroups());
    }
  }

  uploadUserProfileImage(picture: File) {
    if (!picture) {
      return;
    }
    this.imageService
      .uploadProfileImage({ file: picture, userId: this.currentUserId })
      .subscribe({
        next: (result) => {
          this.authService.setUser({
            ...this.authService.getUserValue(),
            profileImageUrl: result,
          });
        },
        error: (err) => {
          this.toastrService.error(
            err.error.detail || 'Something went wrong',
            'Error'
          );
        },
      });
  }

  uploadChatGroupImage(picture: File) {
    if (!picture) {
      return;
    }
    const selectedChatGroupId = this.selectedChat.id;
    this.imageService
      .uploadChatGroupImage({ file: picture, chatGroupId: selectedChatGroupId })
      .subscribe({
        next: (publicImageId) => {
          this._selectedChat.mutate((selectedChat) => {
            selectedChat.profileImageUrl = publicImageId;
          });
          this._sidebarChatGroups.mutate((groups) => {
            groups.find((g) => g.id === selectedChatGroupId)!.profileImageUrl =
              publicImageId;
          });
        },
        error: (err) => {
          this.toastrService.error(
            err.error.detail || 'Something went wrong',
            'Error'
          );
        },
      });
  }

  onDestroy() {
    this.chatHub.disconnectFromHub();
  }
}
