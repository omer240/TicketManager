import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const noAuthGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Kullanıcı zaten giriş yapmışsa tickets sayfasına yönlendir
  if (authService.getCurrentUser() && !authService.isTokenExpired()) {
    router.navigate(['/tickets']);
    return false;
  }

  return true;
};
