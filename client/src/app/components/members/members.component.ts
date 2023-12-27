import { Component, OnInit } from '@angular/core';

import { Member } from 'src/app/Models/member';
import { Pagination } from 'src/app/Models/pagination';
import { UserParams } from 'src/app/Models/userParams';
import { AccountService } from 'src/app/services/account.service';
import { MemberService } from 'src/app/services/member.service';

@Component({
  selector: 'app-members',
  templateUrl: './members.component.html',
  styleUrls: ['./members.component.css'],
})
export class MembersComponent implements OnInit {
  pagination: Pagination | undefined;
  members: Member[] | undefined;

  userParams: UserParams | undefined;

  genders = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' },
  ];

  constructor(
    private memberService: MemberService,
    private accountService: AccountService
  ) {
    this.userParams = this.memberService.getUserParams();
  }
  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    if (!this.userParams) return;
    this.memberService.getMembers(this.userParams).subscribe((response) => {
      if (response.result && response.pagination) {
        this.members = response.result;
        this.pagination = response.pagination;
      }
    });
  }

  resetFilter() {
    this.memberService.resetUserParams();
    this.userParams = this.memberService.getUserParams();
    this.loadMembers();
  }

  pageChanged(event: any) {
    if (this.userParams && this.userParams?.pageNumber !== event.page) {
      this.userParams.pageNumber = event.page;
      this.loadMembers();
    }
  }
}
