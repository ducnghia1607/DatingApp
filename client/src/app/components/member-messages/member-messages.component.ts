import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/Models/message';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, TimeagoModule, FormsModule],
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm!: NgForm;
  @Input() username?: string;
  @Input() messages: Message[] = [];
  messageContent: string = '';

  constructor(private messageService: MessageService) {}
  ngOnInit(): void {}

  sendMessage() {
    if (this.username) {
      this.messageService
        .sendMessage(this.username, this.messageContent)
        .subscribe({
          next: (mes) => {
            this.messages.push(mes);
            this.messageContent = '';

            this.messageForm.reset();
          },
        });
    }
  }
}
