import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const raw = localStorage.getItem('signbridge-auth');
  if (!raw) return next(req);

  try {
    const token = JSON.parse(raw)?.accessToken as string | undefined;
    if (!token) return next(req);

    return next(req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    }));
  } catch {
    return next(req);
  }
};
