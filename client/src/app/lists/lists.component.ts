import { Component, OnInit } from '@angular/core';
import { MemberService } from '../_services/member.service';
import { Member } from '../_models/Member';
import { Pagination } from '../_models/Pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
members : Partial<Member[]>;
predicate = "liked";
pageNumber = 1;
pageSize = 2
pagination : Pagination;


  constructor(private memberServie : MemberService) { }

  ngOnInit(): void {
    this.LoadMember();
  }

  LoadMember(){
    this.memberServie.GetLikes(this.predicate, this.pageNumber, this.pageSize).subscribe((response) => {
      this.members = response.result;
      console.log(this.members);
      this.pagination = response.pagination;
    })
  }

  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.LoadMember();
  }

}
