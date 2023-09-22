import { ActionReducerMap } from '@ngrx/store';
import { AppState } from './app.state';
import { userReducer } from './shared/states/user/user.reducer';

export const appReducer: ActionReducerMap<AppState, any> = {
  user: userReducer,
};
