import { createSelector } from '@ngrx/store';
import { appStateSelector } from 'src/app/app.selector';

export const userSelector = createSelector(
  appStateSelector,
  (state) => state.user
);
