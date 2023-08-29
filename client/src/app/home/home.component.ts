import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  RegisterMode = false;

  constructor() { }

  ngOnInit(): void {
  }

  registerToggle(){
    this.RegisterMode = !this.RegisterMode;
    console.log(this.RegisterMode);
  }

  canceledRegister(event:boolean){
    this.RegisterMode = event;
  }

}
