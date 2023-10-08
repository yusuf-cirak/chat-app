import { UserDto } from './user-dto';

export type CreateHubChatGroupDto = {
  id: string;
  name: string;
  isPrivate: boolean;
  users: { id: string; userName: string; profileImageUrl: string }[];
  profileImageUrl: string;
};
