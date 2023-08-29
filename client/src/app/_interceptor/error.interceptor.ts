import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router, private toast: ToastrService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error) => {
        if (error) {
          switch (error.status) {
            case 400:
              if (error.error.errors) {
                const modalStatError = [];
                for (const key in error.error.errors) {
                  if (error.error.errors[key]) {
                    modalStatError.push(error.error.errors[key]);
                  }
                }
                throw modalStatError.flat();
              }else if(typeof(error.error) === "object"){
                this.toast.error(error.error.title, error.error.status);
              }
              else {
                this.toast.error(error.error, error.status);
              }
              break;
              case 404:
                this.router.navigateByUrl("/not-found");
              break;
              case 401:
                this.toast.error("Unothorized", error.status);
                break;
              case 500:
                const NavigationExtras : NavigationExtras = {state : {error : error.error}};
                this.router.navigateByUrl("/server-error", NavigationExtras);
                break;
            default:
              this.toast.error("something was wrong!!!");
              console.log(error);
              break;
          }
        }
        return throwError(error);
      })
    );
  }
}
