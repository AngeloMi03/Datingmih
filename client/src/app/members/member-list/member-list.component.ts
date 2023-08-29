import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/Member';
import { Pagination } from 'src/app/_models/Pagination';
import { UseParams } from 'src/app/_models/UserParams';
import { User } from 'src/app/_models/Users';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  members: Member[];
  pageNumber = 1;
  pagination: Pagination;
  pageSize = 5;
  useParams: UseParams;
  user: User;
  GenderList = [{value : 'male', display :'male'},{value : 'female', display :'female'}];

  constructor(private servicemember: MemberService ) {
   this.useParams = this.servicemember.GetUserParams();
  }

  ngOnInit(): void {
    this.LoadUser();
  }

  LoadUser() {
    this.servicemember.SetUserParams(this.useParams);
    this.servicemember
      .GetMembers(this.useParams)
      .subscribe((response) => {
        this.members = response.result;
        this.pagination = response.pagination;
      });
  }

  ResetFilter(){
    this.useParams = this.servicemember.ResetUserParams();
    this.LoadUser();
  }

  pageChanged(event: any) {
    this.useParams.pageNumber = event.page;
    this.servicemember.SetUserParams(this.useParams);
    this.LoadUser();
  }
}
