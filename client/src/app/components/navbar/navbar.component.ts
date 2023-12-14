import { Component } from '@angular/core';
import { Observable, of } from 'rxjs';
import { User } from 'src/app/Models/User';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
  providers: [],
})
export class NavbarComponent {
  model: any = {};

  constructor(public accountService: AccountService) {}

  login() {
    this.accountService.login(this.model).subscribe({
      next: (response) => {
        console.log(response);
      },
      error: (error) => {
        console.log(error);
      },
      complete: () => {
        console.log('Completed login');
      },
    });

    console.log(this.model);
  }

  logout() {
    this.accountService.logout();
  }

  // getCurrentUser() {
  //   this.accountService.currentUser$.subscribe({
  //     next: (user) => {
  //       this.isLogin = !!user;
  //     },
  //     error: (err) => console.log(err),
  //   });
  // }
}
