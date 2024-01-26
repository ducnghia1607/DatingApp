import { Component } from '@angular/core';
import { Message } from 'src/app/Models/message';
import { Pagination } from 'src/app/Models/pagination';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css'],
})
export class MessagesComponent {
  pageNumber: number = 1;
  pageSize: number = 5;
  container: string = 'Inbox';
  pagination?: Pagination;
  messages?: Message[];
  loading: boolean = false;
  constructor(private messageService: MessageService) {
    this.loadMessages();
    // console.log(this.messages);
  }

  loadMessages() {
    this.loading = true;
    this.messageService
      .loadMessages(this.pageNumber, this.pageSize, this.container)
      .subscribe((response) => {
        this.pagination = response.pagination;
        this.messages = response.result;
        this.loading = false;
      });
  }

  pageChanged(event: any) {
    // if (!this.likeParams) return;
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }

  deleteMessage(id: number) {
    return this.messageService.deleteMessage(id).subscribe({
      next: () => {
        this.messages?.splice(
          this.messages.findIndex((m) => m.id == id),
          1
        );
      },
    });
  }
}
