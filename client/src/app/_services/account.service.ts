import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject, map, noop } from 'rxjs';
import { User } from '../_models/Users';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = 'https://localhost:7122/api/';
  private CurrentUserSource = new ReplaySubject<User>(1);
  CurrentUser$ = this.CurrentUserSource.asObservable();

  constructor(private http: HttpClient, private presenceHub : PresenceService) {}

  login(model: any) {
    return this.http.post(this.baseUrl + 'Accounte/login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          this.SetCurrentUser(user);
          this.presenceHub.createHubConnection(user);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'Accounte/register', model).pipe(
      map((user: any) => {
        if (user) {
          this.SetCurrentUser(user);
          this.presenceHub.createHubConnection(user);
        }
        return user;
      })
    );
  }

  SetCurrentUser(user: User) {
    user.role = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.role = roles : user.role.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    this.CurrentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.CurrentUserSource.next(null);
    this.presenceHub.stopHubConnexion();
  }

  getDecodedToken(token : string)
  {
      return JSON.parse(atob(token.split('.')[1]));
  }
}
