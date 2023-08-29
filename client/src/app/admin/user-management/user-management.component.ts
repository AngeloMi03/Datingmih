import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from 'src/app/_modals/roles-modal/roles-modal.component';
import { User } from 'src/app/_models/Users';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
   users : Partial<User[]>
   BsModelRef : BsModalRef;

  constructor(private AdminSerive : AdminService, private ModelService : BsModalService) { }

  ngOnInit(): void {
    this.GetUserswithRoles();
  }

  GetUserswithRoles(){
    this.AdminSerive.GetUserwhithRole().subscribe(users =>{
      this.users = users;
    })
  }

  openRolesModal(user : User)
  {
    const config = {
      class : 'modal-dialog-centered',
      initialState  : {
        user,
        roles : this.GetRolesArray(user)
      }
    }
    this.BsModelRef = this.ModelService.show(RolesModalComponent, config)
    this.BsModelRef.content.updateSelecterole.subscribe(values =>{
      const rolesToupdate ={ roles : [...values.filter(el => el.checked === true).map(el => el.name)]}
      console.log("username "  + user.username)
        console.log("rolesToupdate.roles "  +rolesToupdate.roles)

      if(rolesToupdate)
      {
        
        this.AdminSerive.UpdateUserRole(user.username, rolesToupdate.roles).subscribe(() => {
          user.role = [...rolesToupdate.roles]
        })
      }

    })
  }

  private GetRolesArray(user)
  {
    const roles = [];
    const userRoles = user.role
    const avalableRole : any[] = [
      {name : 'Admin' , value : 'Admin'},
      {name : 'Moderator' , value : 'Moderator'},
      {name : 'Member' , value : 'Member'}
    ] 

    avalableRole.forEach(role => {
      let isMatch = false;
      for(const userRole of userRoles)
      {
           if(userRole == role.name)
           {
               isMatch = true;
               role.checked = true;
               roles.push(role)
               break;
           }
      }

      if(!isMatch)
      {
        role.checked = false;
        roles.push(role)
      }
    })

    return roles;
  }
}
