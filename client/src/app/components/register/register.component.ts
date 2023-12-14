import { Component, EventEmitter, Output } from '@angular/core';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  @Output() registerCancel = new EventEmitter();
  model: any = {};

  constructor(private accountService: AccountService) {}

  register() {
    console.log(this.model);
    this.accountService.register(this.model).subscribe({
      next: (res) => {
        this.cancel();
        localStorage.setItem('user', JSON.stringify(res.token));
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  cancel() {
    this.registerCancel.emit(false);
  }
}
