import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../_models/Users';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  baseUrl = 'https://localhost:7122/api/';

  constructor(private http : HttpClient) { }

  GetUserwhithRole(){
    return this.http.get<Partial<User[]>>(this.baseUrl + "Admin/users-with-role")
  }

  UpdateUserRole(username : String, roles : string[]){
    return this.http.post(this.baseUrl + 'Admin/edit-role/' + username + '?roles=' + roles, {})
  }
}
