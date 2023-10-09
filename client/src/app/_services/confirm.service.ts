import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmDialogComponent } from '../_models/confirm-dialog/confirm-dialog.component';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {

  BsModelRef: BsModalRef;

  constructor(private ModelService : BsModalService) { }

  Confirm(title = 'confirmation', message = "are you sure to do this?", btnOKText = "Ok", 
    btnCancelText='Cancel') : Observable<boolean>
  {
     const config = {
      initialState : {
        title,
        message,
        btnOKText,
        btnCancelText
      }
     }

     this.BsModelRef = this.ModelService.show(ConfirmDialogComponent, config);

     return new Observable<boolean>(this.getResult());
  }

  private getResult()
  {
    return (observer) => {
       const subscription = this.BsModelRef.onHidden.subscribe(() =>{
          observer.next(this.BsModelRef.content.result);
          observer.complete();
       });

       return {
          unsubscribe()
          {
            subscription.unsubscribe();
          }
       }
    }


  }
}
