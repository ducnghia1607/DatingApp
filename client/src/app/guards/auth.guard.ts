import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../services/account.service';
import { map } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastService = inject(ToastrService);

  return accountService.currentUser$.pipe(
    map((res) => {
      if (res) return true;
      else {
        toastService.error('You shall not pass');
        return false;
      }
    })
  );
  // var auth: boolean = false;
  // accountService.currentUser$
  //   .pipe(
  //     map((res) => {
  //       if (res) {
  //         return true;
  //       } else {
  //         toastService.error('Unauthorized');
  //         return false;
  //       }
  //     })
  //   )
  //   .subscribe((res) => {
  //     auth = res;
  //   });
  // return auth;
};
