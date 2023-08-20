import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadChildren: () =>
      import('./core/layouts/auth/auth.routes').then((c) => c.authRoutes),
  },
  {
    path: 'chat',
    loadChildren: () =>
      import('./core/layouts/chat/chat.routes').then((c) => c.chatRoutes),
  },
  // TODO: Write welcome component, and add canActivate guard if user not logged in
  // { path: 'chat', canActivate: [true] }, // TODO: Write chat component, and add canActivate guard if user logged in
  {
    path: '**',
    loadComponent: () =>
      import('../app/core/pages/not-found/not-found.component').then(
        (c) => c.NotFoundComponent
      ),
  },
];
