import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthenticationService } from '../../cross-cutting/security/authentication/authentication.service';
import { map, Observable } from 'rxjs';
import { WebRoutes } from '../../cross-cutting/operation-management/model/web-routes.constants';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: AuthenticationService, private router: Router) { }

  canActivate(): Observable<boolean> {
    return this.authService.isAuthenticated().pipe(
      map((authenticated: boolean) => {
        if (authenticated) {
          return true;
        } else {
          this.router.navigate([WebRoutes.login]);
          return false;
        }
      })
    );
  }

}

