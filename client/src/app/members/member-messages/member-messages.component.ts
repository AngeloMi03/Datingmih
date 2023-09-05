import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/Message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  //username: string;
  @Input() messages : Message[];
  @Input() username : string ; 
  contentMessage : string;
  @ViewChild('messageForm')  messageForm : NgForm;

  constructor(public messageService : MessageService) { }

  ngOnInit(): void {
    //this.loadMessage();
  }

  /*loadMessage(){
    this.messageService.GetMessageThread(this.username).subscribe(message =>{
      this.messages = message;
    })
  }*/

  sendMessage(){                                                     //subscribe
    this.messageService.SendMessage(this.username, this.contentMessage).then(
      () => {
        //this.messages.push(message);
        this.messageForm.reset();
      }
    )
  }

}
