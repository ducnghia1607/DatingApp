import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { User } from '../Models/User';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  baseUrl = environment.hubUrl;
  private hubConnection?: HubConnection;

  usersOnlineSource: BehaviorSubject<string[]> = new BehaviorSubject<string[]>(
    []
  );
  usersOnline$ = this.usersOnlineSource.asObservable();

  constructor(private toast: ToastrService) {}

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.baseUrl + 'presence', {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => console.log(error));
    // Registers a handler that will be invoked when the hub method with the specified method name is invoked.
    this.hubConnection.on('UserIsOnline', (username) => {
      this.toast.success(username + 'has connected');
    });

    this.hubConnection.on('UserIsOffline', (username) => {
      this.toast.success(username + 'has disconnected');
    });

    this.hubConnection.on('GetUsersOnline', (userOnlines) => {
      this.usersOnlineSource.next(userOnlines);
    });
  }

  stopHubConnection() {
    this.hubConnection?.stop().catch((error) => console.log(error));
  }
}
