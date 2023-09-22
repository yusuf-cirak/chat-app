import { createReducer, on } from '@ngrx/store';
import { UserDto } from '../../api/user-dto';
import { loginAction, registerAction } from './user.actions';

export const initialState: Readonly<UserDto> = {} as UserDto;

export const userReducer = createReducer(
  initialState,
  on(registerAction, (state, { user }) => {
    return { ...state, ...user };
  }),
  on(loginAction, (state, { user }) => {
    return { ...state, ...user };
  })
);
