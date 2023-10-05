export interface ChatGroupDto {
  id: string;
  name: string;
  isPrivate: boolean;
  userIds: string[];
  profileImageUrl: string;
}
