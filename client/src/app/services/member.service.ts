import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../Models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResult, Pagination } from '../Models/pagination';
import { UserParams } from '../Models/userParams';
import { AccountService } from './account.service';
import { User } from '../Models/User';

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

  getMembers(userParams: UserParams) {
    var params = this.getPaginationHeader(
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

    return this.getPaginatedResult<Member[]>(
      params,
      this.baseUrl + 'users'
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

  private getPaginatedResult<T>(params: HttpParams, url: string) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.http
      .get<T>(url, {
        observe: 'response',
        params,
      })
      .pipe(
        map((response) => {
          if (response.body) {
            paginatedResult.result = response.body;
          }
          const pagination = response.headers.get('Pagination');
          if (pagination) {
            paginatedResult.pagination = JSON.parse(pagination);
          }

          return paginatedResult;
        })
      );
  }

  private getPaginationHeader(
    pageNumber: number | undefined,
    itemsPerPage: number | undefined
  ) {
    var params = new HttpParams();
    if (pageNumber && itemsPerPage) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', itemsPerPage);
    }
    return params;
  }
}
