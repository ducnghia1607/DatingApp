import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  getPaginatedResult,
  getPaginationHeader,
} from '../Models/paginationHelper';
import { environment } from 'src/environments/environment';
import { Message } from '../Models/message';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

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

  sendMessage(username: string, content: string) {
    return this.http.post<Message>(this.baseUrl + 'messages', {
      recipientUsername: username,
      content,
    });
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
