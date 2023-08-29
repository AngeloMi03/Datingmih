import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';
import { User } from '../_models/Users';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  @Input()  appHasRole : String[];
  user : User;

  constructor(private viewContainerRef : ViewContainerRef, 
    private templateRef : TemplateRef<any>, private accounteService : AccountService) { 
      this.accounteService.CurrentUser$.pipe(take(1)).subscribe(user =>{
         this.user = user;
      })
    }

  ngOnInit(): void {
    if(!this.user.role || this.user == null )
    {
      this.viewContainerRef.clear();
      return;
    }

    if(this.user?.role.some(r => this.appHasRole.includes(r)))
    {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    }else{
      this.viewContainerRef.clear();
    }
  }

}
