import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Gallery, GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/Models/member';
import { MemberService } from 'src/app/services/member.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/services/message.service';
import { Message } from 'src/app/Models/message';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    TabsModule,
    GalleryModule,
    TimeagoModule,
    MemberMessagesComponent,
  ],
})
export class MemberDetailComponent implements OnInit {
  // static : true -> initialize immediately
  @ViewChild('tabSet', { static: true }) tabSet?: TabsetComponent;
  tabActive?: TabDirective;
  member: Member = {} as Member;
  images: GalleryItem[] = [];
  messages: Message[] = [];
  queryParams: string = '';
  constructor(
    private route: ActivatedRoute,
    private messageService: MessageService
  ) {}
  ngOnInit(): void {
    this.route.data.subscribe({
      next: (data) => {
        this.member = data['member'];
      },
    });

    this.route.queryParams.subscribe({
      next: (params) => {
        this.queryParams = params['tab'];
        console.log('Hello sir');
        // tabSet == null on this stage
        this.selectTab(this.queryParams);
      },
    });
    this.getImages();
  }

  OnTabActive(data: TabDirective) {
    this.tabActive = data;
    if (this.tabActive.heading === 'Messages') {
      this.loadMessages();
    }
  }

  selectTab(heading: string) {
    if (this.tabSet && this.tabSet.tabs) {
      this.tabSet.tabs.find((x) => x.heading === heading)!.active = true;
    }
  }

  loadMessages() {
    if (this.member) {
      this.messageService.loadMessagesThread(this.member.userName).subscribe({
        next: (response) => {
          this.messages = response;
        },
      });
    }
  }

  getImages() {
    if (!this.member) return;
    for (const photo of this.member.photos) {
      this.images.push(new ImageItem({ src: photo.url, thumb: photo.url }));
    }
  }
}
