export interface CreateChatGroupDto {
  name: string;
  participantUserIds: string[];
  isPrivate: boolean;
}
