import { Component } from '@angular/core';
import { Member } from 'src/app/Models/member';
import { MemberService } from 'src/app/services/member.service';

@Component({
  selector: 'app-members',
  templateUrl: './members.component.html',
  styleUrls: ['./members.component.css'],
})
export class MembersComponent {
  members: Member[] = [];
  constructor(private memberService: MemberService) {
    this.loadMembers();
  }
  loadMembers() {
    this.memberService.getMembers().subscribe((res) => (this.members = res));
  }
}
