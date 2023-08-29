import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/Users';
import { AccountService } from './_services/account.service';
import { MemberService } from './_services/member.service';
import { PresenceService } from './_services/presence.service';

const httpOptions1  = {
  headers : new HttpHeaders({
    Authorization : "Bearer " + JSON.parse(localStorage.getItem("user"))?.token
  })
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {
  title = 'the dating app';
  users : any;

 

  constructor(private http : HttpClient, private accounteService : AccountService, 
         private memberservice : MemberService, private hubSevice : PresenceService){}

  ngOnInit() {
    //this.GetUser();
    this.SetCurrentUser();
  }

  SetCurrentUser(){
    const user : User = JSON.parse(localStorage.getItem("user"))
    if(user)
    {
     this.accounteService.SetCurrentUser(user);
     this.hubSevice.createHubConnection(user);
    }
  }

 /*GetUser(){
    this.memberservice.GetMembers();
  }*/


}
