import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export function roleGuard(...roles: string[]): CanActivateFn {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (!auth.session) {
      return router.parseUrl('/login');
    }

    if (roles.length && !roles.includes(auth.role ?? '')) {
      return router.parseUrl('/login');
    }

    return true;
  };
}
