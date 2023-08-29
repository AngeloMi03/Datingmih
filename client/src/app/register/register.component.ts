import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
@Output() canceledRegister = new EventEmitter();

registerForm : FormGroup;


 model : any = {};
 validationErrors: string[] = [];

  constructor(private accounteService : AccountService, private toast : ToastrService,
    private fb : FormBuilder, private router : Router
    ) { }

  ngOnInit(): void {
    this.initializeForm()
  }
  
  initializeForm(){
    this.registerForm = this.fb.group({
      gender : ['male'],
      username : ['', Validators.required],
      knowAs : ['', Validators.required],
      dateOfBirth : ['', Validators.required],
      city : ['', Validators.required],
      country : ['', Validators.required],
      password : ['', [Validators.required, Validators.minLength(4)]],
      confirmPassword : ['',[Validators.required, this.MatchValue('password')]]
    })
  }

  MatchValue(matchTo : string) : ValidatorFn{
      return (control : AbstractControl ) => {
         return control?.value === control?.parent?.controls[matchTo].value ? null : {isMatching : true}
      }
  }

  register(){
    this.accounteService.register(this.registerForm.value).subscribe(response => {
      this.router.navigateByUrl("/members");
    },error => {
      this.validationErrors = error;
    })
   
    }

  canceled(){
    this.canceledRegister.emit(false);
  }

}
