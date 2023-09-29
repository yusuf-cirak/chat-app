export interface ChatGroupMessagesDto {
  [chatGroupId: string]: MessageDto[];
}

interface MessageDto {
  id: string;
  userId: string;
  chatGroupId: string;
  body: string;
  sentAt: string;
}
