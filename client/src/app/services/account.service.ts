import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../Models/User';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = 'https://localhost:5000/api';

  currentUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSource.asObservable();
  constructor(private http: HttpClient) {}

  setCurrentUser(user: User) {
    this.currentUserSource.next(user);
  }

  login(model: any) {
    return this.http.post<User>(this.baseUrl + '/account/login', model).pipe(
      map((response: User) => {
        var user = response;
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl + '/account/register', model).pipe(
      map((response) => {
        if (response) {
          this.setCurrentUser(response);
        }
        return response;
      })
    );
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
