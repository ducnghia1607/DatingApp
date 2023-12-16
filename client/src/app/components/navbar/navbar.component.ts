import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable, of } from 'rxjs';
import { User } from 'src/app/Models/User';
import { AccountService } from 'src/app/services/account.service';
import { BsDropdownDirective } from 'ngx-bootstrap/dropdown/bs-dropdown.directive';
import { BsDropdownConfig } from 'ngx-bootstrap/dropdown';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
  providers: [
    {
      provide: BsDropdownConfig,
      useValue: { isAnimated: true, autoClose: true },
    },
  ],
})
export class NavbarComponent {
  model: any = {};

  constructor(
    public accountService: AccountService,
    private router: Router,
    private toastService: ToastrService
  ) {}

  login() {
    this.accountService.login(this.model).subscribe({
      next: (response) => {
        console.log(response);
        // this.toastService.error(error.message);

        this.router.navigateByUrl('/members');
      },
      error: (error) => {
        // this.toastService.error('Your account or password is incorrect');
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
    this.router.navigateByUrl('/');
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
