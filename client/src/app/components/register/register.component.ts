import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  @Output() registerCancel = new EventEmitter();
  registerForm!: FormGroup;
  maxDate: Date = new Date();
  validationErrors: string[] = [];
  constructor(
    private accountService: AccountService,
    private toastService: ToastrService,
    private formBuilder: FormBuilder,
    private router: Router
  ) {}

  ngOnInit() {
    this.initializeFormGroup();
  }

  initializeFormGroup() {
    this.registerForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(6),
          Validators.maxLength(12),
        ],
      ],
      confirmPassword: [
        '',
        {
          validators: [Validators.required, this.matchValues('password')],
        },
      ],
      knownAs: ['', Validators.required],
      gender: ['male', Validators.required],
      dateOfBirth: ['', Validators.required],
      country: ['', Validators.required],
      city: ['', Validators.required],
    });
    this.registerForm.controls['password'].valueChanges.subscribe(() => {
      this.registerForm.get('confirmPassword')?.updateValueAndValidity();
    });

    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value
        ? null
        : { notMatching: true };
    };
  }

  register() {
    console.log(this.registerForm.value);
    var dob = this.GetDateOnly(this.registerForm.controls['dateOfBirth'].value);
    const values = { ...this.registerForm.value, dateOfBirth: dob };
    console.log(values);
    this.accountService.register(values).subscribe({
      next: (res) => {
        // Vì accountService đã setCurrentUser ,LocalStorage.setItem  trong method register rồi nên k cần set ở đây nữa
        this.router.navigateByUrl('/members');
        // this.accountService.setCurrentUser(res);
        // localStorage.setItem('user', JSON.stringify(res));
      },
      error: (err) => {
        this.validationErrors = err.error;
      },
    });
  }

  private GetDateOnly(dob: string | undefined) {
    if (!dob) return;
    let theDob = new Date(dob);
    return new Date(
      theDob.setMinutes(theDob.getMinutes() - theDob.getTimezoneOffset())
    )
      .toISOString()
      .slice(0, 10);
  }

  cancel() {
    this.registerCancel.emit(false);
  }
}
