import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/Models/member';
import { MemberService } from 'src/app/services/member.service';

@Component({
  selector: 'app-members',
  templateUrl: './members.component.html',
  styleUrls: ['./members.component.css'],
})
export class MembersComponent {
  members$: Observable<Member[]> | undefined;
  constructor(private memberService: MemberService) {
    this.members$ = this.memberService.getMembers();
  }
}
