import { Component } from '@angular/core';
import { LikeParams } from 'src/app/Models/likeParams';
import { Member } from 'src/app/Models/member';
import { Pagination } from 'src/app/Models/pagination';
import { MemberService } from 'src/app/services/member.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css'],
})
export class ListsComponent {
  members: Member[] = [];
  likeParams: LikeParams = new LikeParams();
  pagination: Pagination | undefined;
  constructor(private memberService: MemberService) {
    this.loadUserLikes();
  }

  loadUserLikes() {
    if (!this.likeParams) return;
    this.memberService.getUserLikes(this.likeParams).subscribe({
      next: (response) => {
        if (response.result) {
          this.members = response.result;
          this.pagination = response.pagination;
        }
      },
    });
  }

  pageChanged(event: any) {
    if (!this.likeParams) return;
    if (this.likeParams.pageNumber !== event.page) {
      this.likeParams.pageNumber = event.page;
      this.loadUserLikes();
    }
  }
}
