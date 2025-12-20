import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const noAuthGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // If user is already authenticated, redirect to tickets
  if (authService.getCurrentUser() && !authService.isTokenExpired()) {
    router.navigate(['/tickets']);
    return false;
  }

  // Not authenticated, allow access to login/register
  return true;
};
