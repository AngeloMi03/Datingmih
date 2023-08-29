import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
   
  constructor(private accounteService  : AccountService , private toast : ToastrService){}

  canActivate( ): Observable<boolean> 
  {
     return this.accounteService.CurrentUser$.pipe(
      map((user): boolean => {
           if(user.role.includes("Admin") || user.role.includes("Moderator"))
           {
            return true;
           }
           this.toast.error("You can not enter this Area");
           return false;
      })
     )
  }
  
}
