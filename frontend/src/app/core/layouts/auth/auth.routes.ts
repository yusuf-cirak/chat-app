import { Routes } from '@angular/router';

export const authRoutes: Routes = [
  {
    path: '',
    canActivate: [() => true],
    loadComponent: () =>
      import('./auth.component').then((e) => e.AuthComponent),
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'login',
      },
      {
        path: 'login',
        loadComponent: () =>
          import('../../components/auth/login/login.component').then(
            (e) => e.LoginComponent
          ),
      },
      {
        path: 'register',
        loadComponent: () =>
          import('../../components/auth/register/register.component').then(
            (e) => e.RegisterComponent
          ),
      },
    ],
  },
];
