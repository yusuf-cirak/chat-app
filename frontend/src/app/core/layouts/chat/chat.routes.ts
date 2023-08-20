export const chatRoutes = [
  {
    path: '',
    canActivate: [() => true],
    loadComponent: () =>
      import('./chat.component').then((m) => m.ChatComponent),
  },
];
