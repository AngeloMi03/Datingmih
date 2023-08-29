import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/Member';
import { User } from 'src/app/_models/Users';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm : NgForm;
  Member : Member;
  user : User;    

  @HostListener("window:beforeunload", ["$event"]) unloadNotification($event:any){
    
  }

  constructor(private accounteService : AccountService, private memberService : MemberService,
       private toast : ToastrService) { 
    this.accounteService.CurrentUser$.pipe(take(1)).subscribe(user => {
      this.user = user
      console.log( "user" + this.user.username)
    });
  }

  ngOnInit(): void {
    this.LoadUser();
  }


  LoadUser(){
    this.memberService.GetMember(this.user.username).subscribe(member => this.Member = member);
  }

  updateUser(){
    //console.log(this.Member);
    this.memberService.UpdateMember(this.Member).subscribe(() =>{
      this.toast.success("profile update successfully")
      this.editForm.reset(this.Member);
    })
  }

}
