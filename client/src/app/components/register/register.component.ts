import { Component, EventEmitter, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  @Output() registerCancel = new EventEmitter();
  model: any = {};

  constructor(
    private accountService: AccountService,
    private toastService: ToastrService
  ) {}

  register() {
    console.log(this.model);
    this.accountService.register(this.model).subscribe({
      next: (res) => {
        this.cancel();
        localStorage.setItem('user', JSON.stringify(res.token));
      },
      error: (err) => {
        console.log(err.error);
        this.toastService.error(err.error.title);
      },
    });
  }

  cancel() {
    this.registerCancel.emit(false);
  }
}
