import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map, of, take } from 'rxjs';
import { Member } from '../_models/Member';
import { PaginatedResult } from '../_models/Pagination';
import { UseParams } from '../_models/UserParams';
import { AccountService } from './account.service';
import { User } from '../_models/Users';
import { getPaginationHeader, getPaginationResult } from './paginationHelper';

const httpOptions = {
  headers: new HttpHeaders({
    Authorization: 'Bearer ' + JSON.parse(localStorage.getItem('user'))?.token,
  }),
};

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  baseUrl = 'https://localhost:7122/api/';
  members: Member[] = [];
  memberCache = new Map();
  user :User;
  useParams : UseParams;


  constructor(private http: HttpClient, private accounteService : AccountService) {
    this.accounteService.CurrentUser$.pipe(take(1)).subscribe((user) => {
      this.user = user;
      this.useParams = new UseParams(this.user);
    });
  }

  GetMembers(UseParams: UseParams) {
    //if(this.members.length > 0) return of(this.members);
    var response = this.memberCache.get(Object.values(UseParams).join('-'));
    if (response) {
      return of(response);
    }

    let params = getPaginationHeader(
      UseParams.pageNumber,
      UseParams.pageSize
    );

    params = params.append('minAge', UseParams.minAge);
    params = params.append('maxAge', UseParams.maxAge);
    params = params.append('gender', UseParams.gender);
    params = params.append('orderBy', UseParams.orderBy);

    console.log(params);
    return getPaginationResult<Member[]>(
      this.baseUrl + 'Users',
      params,this.http
    ).pipe(
      map((response) => {
        this.memberCache.set(Object.values(UseParams).join('-'), response);
        return response;
      })
    );
  }

  GetMember(username: string) {
    //const member = this.members.find(x => x.username === username);
    //if(member !== undefined) return of(member);

    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.username == username);

      if(member){
        return of(member);
      }

    return this.http.get<Member>(this.baseUrl + 'Users/' + username);
  }

  UpdateMember(member: Member) {
    return this.http.put(this.baseUrl + 'Users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }

  SetMainPhoto(photoId: Number) {
    return this.http.put(this.baseUrl + 'Users/set-main-photo/' + photoId, {});
  }

  DeletePhoto(photoId: Number) {
    return this.http.delete(this.baseUrl + 'Users/delete-photo/' + photoId);
  }

  GetUserParams(){
    return this.useParams;
  }

  SetUserParams(params : UseParams){
    this.useParams = params;
  }

  ResetUserParams(){
    this.useParams = new UseParams(this.user);
    return this.useParams;
  }

  Addlikes(member : Member){
    return this.http.post(this.baseUrl + "Likes/" + member.username, {});
  }

  GetLikes(predicate : string, pageNumber: number, pageSize :number){
    let params = getPaginationHeader(pageNumber, pageSize)
    params = params.append("predicate", predicate)
    return getPaginationResult<Partial<Member[]>>(this.baseUrl + "Likes",params, this.http);
  }

  


}
