export interface ChatGroupMessagesDto {
  [chatGroupId: string]: MessageDto[];
}

export interface MessageDto {
  id: string;
  userId: string;
  chatGroupId: string;
  body: string;
  sentAt: string;
}
