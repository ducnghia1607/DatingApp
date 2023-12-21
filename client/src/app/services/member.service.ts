import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../Models/member';
import { map, of } from 'rxjs';
import { Photo } from '../Models/photo';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  constructor(private http: HttpClient) {}

  getMembers() {
    if (this.members.length > 0) return of(this.members);
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map((res) => {
        this.members = res;
        return res;
      })
    );
  }

  getMember(username: string) {
    const member = this.members.find((m) => m.userName == username);
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
