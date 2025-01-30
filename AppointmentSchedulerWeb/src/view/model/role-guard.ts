import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthenticationService } from '../../cross-cutting/security/authentication/authentication.service';
import { RoleType } from '../../view-model/business-entities/types/role.types';
import { AccountData } from '../../view-model/business-entities/account';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class RoleGuard implements CanActivate {

  constructor(private authService: AuthenticationService, private router: Router) { }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    const allowedRoles: RoleType[] = next.data['roles'] || [];

    return this.authService.getAccountData().pipe(
      map((accountData: AccountData | null) => {
        if (accountData && allowedRoles.includes(accountData.role)) {
          return true;
        }
        return false;
      })
    )
  }
}

