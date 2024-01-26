import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { User } from '../Models/User';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

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

  constructor(private toast: ToastrService, private router: Router) {}

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
      this.usersOnline$.pipe(take(1)).subscribe((usersOnline) => {
        this.usersOnlineSource.next([...usersOnline, username]);
      });
    });

    this.hubConnection.on('UserIsOffline', (username) => {
      this.usersOnline$.pipe(take(1)).subscribe((usersOnline) => {
        this.usersOnlineSource.next(usersOnline.filter((x) => x != username));
      });
    });

    this.hubConnection.on('GetUsersOnline', (userOnlines) => {
      this.usersOnlineSource.next(userOnlines);
    });

    this.hubConnection.on('NewMessageReceived', ({ username, knowAs }) => {
      this.toast
        .info(knowAs + 'sent you new message ! Click to see it now')
        .onTap.pipe(take(1))
        .subscribe(() => {
          this.router.navigateByUrl('/members/' + username + '?tab=Messages');
        });
    });
  }

  stopHubConnection() {
    this.hubConnection?.stop().catch((error) => console.log(error));
  }
}
