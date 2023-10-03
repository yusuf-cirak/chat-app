import { UserDto } from 'src/app/core/dtos/user-dto';

export interface LookupItem {
  key: string;
  value: string | number | UserDto;
}
