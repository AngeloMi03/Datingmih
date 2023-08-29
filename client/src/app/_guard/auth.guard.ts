import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import {  map } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private accoutservice : AccountService, private toast : ToastrService){}
  canActivate(): Observable<boolean> {
    return this.accoutservice.CurrentUser$.pipe(
      map((us): boolean => {
       if(us){ 
        return true;
        }
        this.toast.error("you need to login!!");
        return false;
      })
    );
  }
}
