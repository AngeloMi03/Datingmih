import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { map, take } from 'rxjs';
import { Member } from 'src/app/_models/Member';
import { Message } from 'src/app/_models/Message';
import { User } from 'src/app/_models/Users';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild("memberTab", {static : true}) memberTab : TabsetComponent;

  Member : Member;
  GalleryOptions : NgxGalleryOptions[];
  GalleryImages : NgxGalleryImage[];
  activedTad :  TabDirective;
  messages : Message[] = [];
  user : User;



  constructor(public presence : PresenceService, private route : ActivatedRoute,
    private messageService : MessageService, private AccountService : AccountService ) 
    { 
      this.AccountService.CurrentUser$.pipe(take(1)).subscribe(user => this.user = user);
    }
  

  ngOnInit(): void {
    //this.LoadMember();

    this.route.data.subscribe(data =>{
      console.log("data" +  data['member'])
      this.Member = data['member'];
    })

    this.route.queryParams.subscribe(params =>
    {
      params['tab'] ? this.selectTab(params['tab']) : this.selectTab(0) 
    })

    this.GalleryOptions = [
      {
        width : '500px',
        height : '500px',
        imagePercent : 100,
        thumbnailsColumns : 4,
        imageAnimation : NgxGalleryAnimation.Slide,
        preview : false
      }
    ]

    this.GalleryImages =  this.GetImage();

  }

  GetImage() : NgxGalleryImage[]{
    const imageUrl = [];
    for(const photo of this.Member.photos){
      imageUrl.push({
        small : photo.url,
        medium :  photo.url,
        big : photo.url
      })
    }

    return imageUrl;
  }


  /*
  LoadMember(){
    this.memberServie.GetMember(this.route.snapshot.paramMap.get("username")).subscribe(member =>
        {
          this.Member = member;
          this.GalleryImages =  this.GetImage();
        }
      );
  }*/

  loadMessage(){
    this.messageService.GetMessageThread(this.Member.username).subscribe(message =>{
      this.messages = message;
    })
  }

  OnTabActivated(data: TabDirective){
    this.activedTad = data;
    if(this.activedTad.heading == "Message" && this.messages.length == 0){
       //this.loadMessage();
       this.messageService.CreateHubConnection(this.user, this.Member.username);
    }else{
      this.messageService.StopHubConnexion();
    }
  }

  selectTab(tabsId : number){
    this.memberTab.tabs[tabsId].active = true;
  }

  ngOnDestroy(): void {
    this.messageService.StopHubConnexion();
  }

}
