import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-erros',
  templateUrl: './test-erros.component.html',
  styleUrls: ['./test-erros.component.css']
})
export class TestErrosComponent implements OnInit {

  baseUrl = 'https://localhost:7122/api/';
  validationError : string[] = [];

  constructor(private http : HttpClient) { }

  ngOnInit(): void {
  }

  Get404Error(){
    this.http.get(this.baseUrl + "Buggy/not-found").subscribe(response =>{
      console.log(response);
    },error =>{
      console.log(error);
    })  
  }

  Get400Error(){
    this.http.get(this.baseUrl + "Buggy/bad-request").subscribe(response =>{
      console.log(response);
    },error =>{;
      console.log(error);
    })
  }

  Get500Error(){
    this.http.get(this.baseUrl + "Buggy/server-error").subscribe(response =>{
      console.log(response);
    },error =>{
      console.log(error);
    })
  }

  Get401Error(){
    this.http.get(this.baseUrl + "Buggy/auth").subscribe(response =>{
      console.log(response);
    },error =>{
      console.log(error);
    })
  }

  Get400ValidationError(){
    this.http.post(this.baseUrl + "Accounte/Register", {}).subscribe(response =>{
      console.log(response);
    },error =>{
      console.log(error);
      this.validationError = error;
    })
  }
}
