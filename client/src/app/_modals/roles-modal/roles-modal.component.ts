import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/Users';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit {
  @Input() updateSelecterole = new EventEmitter();

  user:User;
  roles : any[];
   //username = this.user.userName

  constructor(public bsModalREf : BsModalRef) { }

  ngOnInit(): void {
  }

  updatedRole(){
    this.updateSelecterole.emit(this.roles);
    this.bsModalREf.hide();
  }

}
