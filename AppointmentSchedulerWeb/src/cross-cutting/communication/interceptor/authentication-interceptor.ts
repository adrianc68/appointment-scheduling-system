import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { map, Observable, switchMap, take, tap } from 'rxjs';
import { AuthenticationService } from '../../security/authentication/authentication.service';
import { UserCredentialsJwt } from '../../../view-model/business-entities/user-credentials-jwt';
import { ApiRoutes } from '../../operation-management/model/api-routes.constants';

@Injectable()
export class AuthenticationInterceptor implements HttpInterceptor {
  constructor(private authService: AuthenticationService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (req.headers.has('Authorization')) {
      console.log("Already has authorization");
      return next.handle(req);
    }

    if (!this.UrlRequiresAuthorization(req.url)) {
      return next.handle(req);
    }

    return this.authService.getCredentials().pipe(
      take(1),
      switchMap((credentials: UserCredentialsJwt | null) => {
        if (credentials != null && credentials.accessToken) {
          const cloneReq = req.clone({
            setHeaders: {
              Authorization: `Bearer ${credentials.accessToken}`,
            },
          });
          return next.handle(cloneReq);
        } else {
          return next.handle(req);
        }
      })
    );
  }

  private UrlRequiresAuthorization(url: string): boolean {
    return Object.values(ApiRoutes).some(route => url.includes(route) && route !== ApiRoutes.login);

  }

}


