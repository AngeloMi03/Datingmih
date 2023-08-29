import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/Users';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accounteServcice : AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let CurrentUser : User;

    this.accounteServcice.CurrentUser$.pipe(take(1)).subscribe(user => CurrentUser = user);
    if(CurrentUser){
      request = request.clone({
        setHeaders : {
          Authorization : `Bearer ${CurrentUser.token}`
        }
      })
    }

    return next.handle(request);
  }
}
