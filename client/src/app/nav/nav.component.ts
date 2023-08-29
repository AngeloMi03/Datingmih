import { Component,OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/Users';
import { map } from 'rxjs/operators';


@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent {

  model: any= {};
 
  //loggedIn : boolean = false;

  constructor( public accounteService : AccountService, private route: Router, private toast: ToastrService){}

  ngOnInit():void {
    //this.GetCurrentUser() 
  }



  login(){
    this.accounteService.login(this.model).subscribe(response => {
      console.log(response)
      this.route.navigateByUrl("/members");
      //this.loggedIn = true;
    },error => {
      console.log(error)
      this.toast.error(error.error);
    })
//console.log(this.model)
  }

  logout(){
    this.accounteService.logout();
    this.route.navigateByUrl("/");
    //this.loggedIn = false;
  }

  /*GetCurrentUser(){
    this.accounteService.CurrentUser$.subscribe(user => {
      this.loggedIn = !!user
      console.log(user);
    } ,error => {
      console.log(error)
    })
  }*/
}
