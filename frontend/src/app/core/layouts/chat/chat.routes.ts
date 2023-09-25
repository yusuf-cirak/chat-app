import { authGuard } from '../../guards/auth.guard';

export const chatRoutes = [
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./chat.component').then((m) => m.ChatComponent),
  },
];
