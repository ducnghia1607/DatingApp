import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent {
  registerMode: boolean = false;
  users: any;

  constructor(private http: HttpClient) {
    this.getUsers();
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  registerCancel(mode: boolean) {
    this.registerMode = mode;
  }

  getUsers() {
    this.http.get<any>('https://localhost:5000/api/users').subscribe({
      next: (response) => {
        this.users = response;
      },
      error: (error) => {
        console.log(error);
      },
      complete: () => {
        console.log('This resquest is completed');
      },
    });
  }
}
