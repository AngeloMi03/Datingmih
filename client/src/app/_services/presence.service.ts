import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/Users';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  hubUrl = "https://localhost:7122/hubs/";
  private hubConnection : HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUser$ = this.onlineUserSource.asObservable();

  constructor(private toast : ToastrService) { }

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
        this.toast.info(username + "Has connected")
      })

      this.hubConnection.on("UserIsOffline", username => {
        this.toast.warning(username + "has disconnected")
      })

      this.hubConnection.on('UserIsOnline', (username : string[]) =>{
        this.onlineUserSource.next(username);
      })
  }

  stopHubConnexion()
  {
    this.hubConnection.stop().catch(error => console.log(error));
  }
}
