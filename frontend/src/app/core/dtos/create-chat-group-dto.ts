export interface CreateChatGroupDto {
  name: string;
  participantUserIds: Set<string>;
  isPrivate: boolean;
}
