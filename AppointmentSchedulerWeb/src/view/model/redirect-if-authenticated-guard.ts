import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { AuthenticationService } from '../../cross-cutting/security/authentication/authentication.service';

@Injectable({
  providedIn: 'root'
})
export class RedirectIfAuthenticatedGuard implements CanActivate {
  constructor(private authService: AuthenticationService, private router: Router) { }

  canActivate(): Observable<boolean> {
    return this.authService.isAuthenticated().pipe(
      tap((isAuth) => {
        if (isAuth) {
          this.router.navigate(['/']);
        }
      }),
      map(isAuth => !isAuth)
    );
  }
}

