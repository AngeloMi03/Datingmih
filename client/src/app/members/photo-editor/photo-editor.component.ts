import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/Member';
import { User } from 'src/app/_models/Users';
import { Photo } from 'src/app/_models/Photo';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
@Input() member:Member;

uploader : FileUploader;
hasBaseDropzoneHover = false;
baseUrl = 'https://localhost:7122/api/';
user : User;

  constructor(private accounteService : AccountService, private memberService : MemberService, 
    private toast : ToastrService
    ) {
    this.accounteService.CurrentUser$.pipe(take(1)).subscribe(user => this.user = user);
   }

  ngOnInit(): void {
    this.initializeUploader()
  }

  fileOverBase(e : any){
    this.hasBaseDropzoneHover = e;
  }

  initializeUploader(){
    this.uploader = new FileUploader({
      url : this.baseUrl + "Users/add-photo",
      authToken : "Bearer " + this.user.token,
      isHTML5 : true,
      allowedFileType : ['image'],
      removeAfterUpload : true,
      autoUpload : false,
      maxFileSize : 10 * 1024 * 1024
    })

    

    this.uploader.onAfterAddingFile = (file) => {
          file.withCredentials = false;
    }

    this.uploader.onSuccessItem = (item,response,status,headers) => {
      if(response){
        const photo : Photo = JSON.parse(response);
        this.member.photos.push(photo);
        if(photo.isMain){
          this.user.PhotoUrl = photo.url;
          this.member.photoUrl = photo.url;
          this.accounteService.SetCurrentUser(this.user);
        }
      }
    }
  }

  MainPhoto(photo : Photo){
      this.memberService.SetMainPhoto(photo.id).subscribe(() => {
        this.user.PhotoUrl = photo.url;
        this.accounteService.SetCurrentUser(this.user);
        this.member.photoUrl = photo.url;
        this.member.photos.forEach(p => {
          if(p.isMain) p.isMain = false;
          if(p.id == photo.id  ) p.isMain = true;
        })
      })
  }

  DeletePhoto(photoId : Number){
    this.memberService.DeletePhoto(photoId).subscribe(() => {
      this.member.photos = this.member.photos.filter(p => p.id != photoId);
      this.toast.success("Photo Deleted successfuly");
    })
  }

 

}
