import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { noAuthGuard } from './core/guards/no-auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/auth/login',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    canActivate: [noAuthGuard],
    children: [
      {
        path: 'login',
        loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
      },
      {
        path: 'register',
        loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
      }
    ]
  },
  {
    path: 'tickets',
    canActivate: [authGuard],
    loadComponent: () => import('./features/tickets/tickets.component').then(m => m.TicketsComponent),
    children: [
      {
        path: '',
        loadComponent: () => import('./features/tickets/pages/ticket-list/ticket-list.component').then(m => m.TicketListComponent)
      },
      {
        path: 'create',
        loadComponent: () => import('./features/tickets/pages/ticket-create/ticket-create.component').then(m => m.TicketCreateComponent)
      },
      {
        path: ':id',
        loadComponent: () => import('./features/tickets/pages/ticket-detail/ticket-detail.component').then(m => m.TicketDetailComponent)
      }
    ]
  },
  {
    path: '**',
    redirectTo: '/auth/login'
  }
];
