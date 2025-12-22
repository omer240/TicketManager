import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Kullanıcı giriş yapmış ve token geçerli mi kontrol et
  if (authService.getCurrentUser() && !authService.isTokenExpired()) {
    return true;
  }

  // Giriş yapılmamış, login sayfasına yönlendir ve dönüş URL'ini sakla
  router.navigate(['/auth/login'], {
    queryParams: { returnUrl: state.url }
  });
  return false;
};
