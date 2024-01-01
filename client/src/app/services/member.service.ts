import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../Models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResult, Pagination } from '../Models/pagination';
import { UserParams } from '../Models/userParams';
import { AccountService } from './account.service';
import { User } from '../Models/User';
import { LikeParams } from '../Models/likeParams';
import {
  getPaginatedResult,
  getPaginationHeader,
} from '../Models/paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  baseUrl = environment.apiUrl;
  pagination: Pagination | undefined;
  paginatedResult = new PaginatedResult<Member[]>();

  memberCache = new Map();
  members: Member[] = [];
  userParams: UserParams | undefined;

  user: User | undefined;

  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (res) => {
        if (res) {
          this.user = res;
          this.setUserParams(new UserParams(this.user));
        }
      },
    });
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  getUserParams() {
    return this.userParams;
  }

  resetUserParams() {
    if (this.user) {
      console.log('In reset userparams ');
      this.setUserParams(new UserParams(this.user));
    }
  }

  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  getUserLikes(likeParams: LikeParams) {
    var params = getPaginationHeader(
      likeParams?.pageNumber,
      likeParams?.pageSize
    );
    params = params.append('predicate', likeParams.predicate);

    return getPaginatedResult<Member[]>(
      params,
      this.baseUrl + 'likes',
      this.http
    );
  }

  getMembers(userParams: UserParams) {
    var params = getPaginationHeader(
      userParams.pageNumber,
      userParams.pageSize
    );

    params = params.append('gender', userParams.gender);
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);

    const key = Object.values(userParams).join('-');
    const response = this.memberCache.get(key);
    if (response) {
      return of(response);
    }

    return getPaginatedResult<Member[]>(
      params,
      this.baseUrl + 'users',
      this.http
    ).pipe(
      map((response) => {
        if (response) {
          this.memberCache.set(key, response);
        }
        return response;
      })
    );
  }

  getMember(username: string) {
    // return object values of map this.memberCache.values()
    // convert object -> array of objects  :  [...this.memberCache.values()];
    // using reduce initialValue = [ ];
    const member = [...this.memberCache.values()]
      .reduce((initialValue, currentValue) => {
        return initialValue.concat(currentValue.result);
      }, [])
      .find((x: Member) => x.userName == username);

    if (member != null) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put<Member>(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        // update member in list members
        // ...this.members[index] -> convert to object and assign = member
        this.members[index] = { ...this.members[index], ...member };
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(
      this.baseUrl + 'users/' + 'set-main-photo/' + photoId,
      {}
    );
  }

  deletePhoto(photoId: number) {
    return this.http.delete(
      this.baseUrl + 'users/' + 'delete-photo/' + photoId
    );
  }
}
