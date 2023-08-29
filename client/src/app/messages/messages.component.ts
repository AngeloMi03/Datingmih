import { Component, OnInit } from '@angular/core';
import { MessageService } from '../_services/message.service';
import { Message } from '../_models/Message';
import { Pagination } from '../_models/Pagination';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages : Message[] = [];
  pagination : Pagination;
  container = "Unread";
  pageSize = 5;
  pageNumber = 1;
  loading = false;
  

  constructor(private messageService : MessageService) { }

  ngOnInit(): void {
    this.loadMessage();
  }

  loadMessage()
  {
    this.loading = true;
    this.messageService.GetMessages(this.pageNumber,this.pageSize,this.container).subscribe(
       (response) => {
        this.messages = response.result;
        this.pagination = response.pagination;
        this.loading = false
       }
    )
  }

  deleteMessage(id : number)
  {
    this.messageService.DeleteMessage(id).subscribe(() => {
      this.messages.splice(this.messages.findIndex(m => m.id == id), 1);
    })
  }

  pageChanged(event:any){
    this.pageNumber = event.page;
    this.loadMessage();
  }
}
