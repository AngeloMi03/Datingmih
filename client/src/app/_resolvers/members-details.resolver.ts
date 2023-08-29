import { Injectable } from "@angular/core";
import { Member } from "../_models/Member";
import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Observable } from "rxjs";
import { MemberService } from "../_services/member.service";


@Injectable({
    providedIn : "root"
})
export class MemberDetailsResolvers implements Resolve<Member>
{

    constructor(private memberService : MemberService){}

    resolve(route : ActivatedRouteSnapshot): Observable<any>
    {
        console.log("username" +route.paramMap.get('username'))
       return this.memberService.GetMember(route.paramMap.get('username'));
    }

}