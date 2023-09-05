import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/Users';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  hubUrl = "https://localhost:7122/hubs/";
  private hubConnection : HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUser$ = this.onlineUserSource.asObservable();

  constructor(private toast : ToastrService , private router : Router) { }

  createHubConnection(user : User)
  {
      this.hubConnection = new HubConnectionBuilder()
       .withUrl(this.hubUrl  + "presence" , {
         accessTokenFactory : () => user.token
       })
       .withAutomaticReconnect()
       .build()
      
      this.hubConnection
       .start()
       .catch(error => console.log(error))

      this.hubConnection.on("UserIsOnline", username => {
        //this.toast.info(username + "Has connected")
        this.onlineUser$.pipe(take(1)).subscribe(usernames => {
          this.onlineUserSource.next([...usernames, username]);
        })
      })

      this.hubConnection.on("UserIsOffline", username => {
        //this.toast.warning(username + "has disconnected")
        this.onlineUser$.pipe(take(1)).subscribe(usernames => {
          this.onlineUserSource.next([...usernames.filter(u => u !== username)])
        })
      })

      this.hubConnection.on('UserIsOnline', (username : string[]) =>{
        this.onlineUserSource.next(username);
      })

      this.hubConnection.on('newMessageReceived',({username, knowAs}) =>{
        this.toast.info(knowAs + ' has sent you a message')
         .onTap
         .pipe(take(1))
         .subscribe(() => {
            this.router.navigateByUrl('/member' + username + '?tab=3')
         })
      })
  }

  stopHubConnexion()
  {
    this.hubConnection.stop().catch(error => console.log(error));
  }
}
