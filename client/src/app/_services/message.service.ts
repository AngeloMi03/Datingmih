import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { getPaginationHeader, getPaginationResult } from './paginationHelper';
import { Message } from '../_models/Message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/Users';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = 'https://localhost:7122/api/';

  hubUrl = "https://localhost:7122/hubs/";
  private hubConnection : HubConnection;

  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http : HttpClient) { }

  CreateHubConnection(user:User, otherUser : String)
  {
     this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUser, {
        accessTokenFactory : () => user.token
      })
      .withAutomaticReconnect()
      .build()

      this.hubConnection.start().catch(error => console.log(error))

      this.hubConnection.on("ReceiveMessageThread", messages =>{
        this.messageThreadSource.next(messages);
      })
  }

  StopHubConnexion()
  {
    if(this.hubConnection){
      this.hubConnection.stop();
    }
  }

  GetMessages(pageNumber, pageSize, Container)
  {
    let params = getPaginationHeader(pageNumber,pageSize);
    params = params.append("Container", Container);

    return getPaginationResult<Message[]>(this.baseUrl + "Message", params, this.http);
  }

  GetMessageThread(username : String)
  {
    return this.http.get<Message[]>(this.baseUrl + "Message/thread/" + username);
  }

  SendMessage(username : string, Content : string){
    return this.http.post<Message>(this.baseUrl + "Message", {RecipientUsername : username, Content} )
  }

  DeleteMessage(id : number)
  {
    return this.http.delete<Message>(this.baseUrl + "Message/" + id);
  }

  
}
