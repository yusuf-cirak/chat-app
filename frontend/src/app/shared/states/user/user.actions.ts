import { createAction, props } from '@ngrx/store';
import { UserDto } from '../../api/user-dto';

export const registerAction = createAction(
  '[User] Register',
  props<{ user: UserDto }>()
);

export const loginAction = createAction(
  '[User] Login',
  props<{ user: UserDto }>()
);
