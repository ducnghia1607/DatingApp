import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  getPaginatedResult,
  getPaginationHeader,
} from '../Models/paginationHelper';
import { environment } from 'src/environments/environment';
import { Message } from '../Models/message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, take } from 'rxjs';
import { User } from '../Models/User';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();
  hubConnection!: HubConnection;

  constructor(private http: HttpClient) {}

  createHubConnection(user: User, otherUsername: string) {
    // ?user = để khi truy cập đến endpoint thì bên phía MessageHub có thể truy cập vào query để lấy username
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();
    this.hubConnection.start().catch((error) => console.log(error));

    this.hubConnection.on('ReceiveMessageThread', (messages) => {
      this.messageThreadSource.next(messages);
    });

    this.hubConnection.on('NewMessage', (message) => {
      this.messageThread$.pipe(take(1)).subscribe({
        next: (messages) => {
          this.messageThreadSource.next([...messages, message]);
        },
      });
    });
  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection?.stop().catch((error) => console.log(error));
    }
  }

  loadMessages(pageNumber: number, pageSize: number, container: string) {
    var params = getPaginationHeader(pageNumber, pageSize);
    params = params.append('container', container);
    return getPaginatedResult<Message[]>(
      params,
      this.baseUrl + 'messages',
      this.http
    );
  }

  loadMessagesThread(username: string) {
    return this.http.get<Message[]>(
      this.baseUrl + 'messages/thread/' + username
    );
  }

  // async sendMessage(...
  //   This way we ensure we return a promise here.
  async sendMessage(username: string, content: string) {
    return this.hubConnection
      ?.invoke('SendMessages', {
        recipientUsername: username,
        content: content,
      })
      .catch((error) => console.log(error));
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
